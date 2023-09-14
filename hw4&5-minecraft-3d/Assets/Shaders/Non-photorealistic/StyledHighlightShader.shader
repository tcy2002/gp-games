Shader "Unlit/StyledHighlightShader"
{
    Properties
    {
        _MainTex ("Main Teuture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _RampTex ("Ramp Texture", 2D) = "white" {}
        _Specular ("Specular", Color) = (1, 1, 1, 1)
        _SpecularScale ("Specular Scale", Range(0, 0.05)) = 0.01
        _TranslationX ("Translation X", Range(-1, 1)) = 0
        _TranslationY ("Translation Y", Range(-1, 1)) = 0
        _RotationX ("Rotation X", Range(-180, 180)) = 0
        _RotationY ("Rotation Y", Range(-180, 180)) = 0
        _RotationZ ("Rotation Z", Range(-180, 180)) = 0
        _ScaleX ("Scale X", Range(-1, 1)) = 0
        _ScaleY ("Scale Y", Range(-1, 1)) = 0
        _SplitX ("Split X", Range(0, 1)) = 0
        _SplitY ("Split Y", Range(0, 1)) = 0
        _SquareN ("Square N", Range(1, 10)) = 1
        _SquareScale ("Square Scale", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "UnityShaderVariables.cginc"

            #define DegreeToRadian 0.0174533

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NormalMap;
            float4 _NormalMap_ST;
            sampler2D _RampTex;
            float4 _Specular;
            float _SpecularScale;
            float _TranslationX;
            float _TranslationY;
            float _RotationX;
            float _RotationY;
            float _RotationZ;
            float _ScaleX;
            float _ScaleY;
            float _SplitX;
            float _SplitY;
            float _SquareN;
            fixed _SquareScale;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 localPos : TEXCOORD2;
                float3 tangentNormal : TEXCOORD3;
                float3 tangentLightDir : TEXCOORD4;
                float3 tangentViewDir : TEXCOORD5;
                SHADOW_COORDS(6)
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.localPos = v.vertex;

                //将所有坐标转换到切线空间中进行计算
                TANGENT_SPACE_ROTATION;
                o.tangentNormal = mul(rotation, v.normal);
                o.tangentLightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
                o.tangentViewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
                
                o.uv = v.uv;
                TRANSFER_SHADOW(o);
                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                //将所有坐标转换到切线空间中进行计算
                fixed3 tangentNormal = normalize(UnpackNormal(tex2D(_NormalMap, i.uv.xy * _NormalMap_ST.xy + _NormalMap_ST.zw)));
                fixed3 tangentLightDir = normalize(i.tangentLightDir);
                fixed3 tangentViewDir = normalize(i.tangentViewDir);
                fixed3 tangentHalfDir = normalize(tangentViewDir + tangentLightDir);

                //光斑大小（与方向有关）
                tangentHalfDir = tangentHalfDir - _ScaleX * tangentHalfDir.x * fixed3(1, 0, 0);
                tangentHalfDir = normalize(tangentHalfDir);
                tangentHalfDir = tangentHalfDir - _ScaleY * tangentHalfDir.y * fixed3(0, 1, 0);
                tangentHalfDir = normalize(tangentHalfDir);

                //旋转
                float xRad = _RotationX * DegreeToRadian;
                float3x3 xRotation = float3x3(1, 0, 0,
                                              0, cos(xRad), sin(xRad),
                                              0, -sin(xRad), cos(xRad));
                float yRad = _RotationY * DegreeToRadian;
                float3x3 yRotation = float3x3(cos(yRad), 0, -sin(yRad),
                                              0, 1, 0,
                                              sin(yRad), 0, cos(yRad));
                float zRad = _RotationZ * DegreeToRadian;
                float3x3 zRotation = float3x3(cos(zRad), sin(zRad), 0,
                                              -sin(zRad), cos(zRad), 0,
                                              0, 0, 1);
                tangentHalfDir = mul(zRotation, mul(yRotation, mul(xRotation, tangentHalfDir)));

                //平移
                tangentHalfDir = tangentHalfDir + fixed3(_TranslationX, _TranslationY, 0);
                tangentHalfDir = normalize(tangentHalfDir);

                //分割
                fixed signX = 1;
                if (tangentHalfDir.x < 0)
                {
                    signX = -1;
                }
                fixed signY = 1;
                if (tangentHalfDir.y < 0)
                {
                    signY = -1;
                }
                tangentHalfDir = tangentHalfDir - _SplitX * signX * fixed3(1, 0, 0) - _SplitY * signY * fixed3(0, 1, 0);
                tangentHalfDir = normalize(tangentHalfDir);

                //方形
                float sqrThetaX = acos(tangentHalfDir.x);
                float sqrThetaY = acos(tangentHalfDir.y);
                fixed sqrnormX = sin(pow(2 * sqrThetaX, _SquareN));
                fixed sqrnormY = sin(pow(2 * sqrThetaY, _SquareN));
                tangentHalfDir -= _SquareScale * (sqrnormX * tangentHalfDir.x * fixed3(1, 0, 0) + sqrnormY * tangentHalfDir.y * fixed3(0, 1, 0));
                tangentHalfDir = normalize(tangentHalfDir);

                //表面颜色
                float4 albedo = tex2D(_MainTex, i.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw);

                //环境光
                float4 ambient = UNITY_LIGHTMODEL_AMBIENT * albedo;

                //漫反射
                float nDotL = dot(tangentNormal, tangentLightDir);
                float4 ramp = tex2D(_RampTex, float2(nDotL * 0.5 + 0.5, 0.5));
                float4 diffuse = _LightColor0 * albedo * ramp;

                //镜面高光
                float spec = dot(tangentNormal, tangentHalfDir);
                float w = fwidth(spec) * 1.0;
                float4 specular = lerp(float4(0, 0, 0, 0), _Specular, smoothstep(-w, w, spec + _SpecularScale - 1));

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                return float4((ambient + (diffuse + specular) * atten).rgb, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}