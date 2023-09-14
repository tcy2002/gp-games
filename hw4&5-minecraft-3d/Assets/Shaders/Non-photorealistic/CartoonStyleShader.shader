Shader "Unlit/CartoonStyleShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _WarpTex("Warp Texture", 2D) = "white" {}
        _SpecExp("Specular Exponent", Range(0.1, 150)) = 20.0
        _SpecBoost("Specular Boost", Float) = 1.0
        _RimExp("Rim Light Exponent", Range(0.1, 150)) = 4.0
        _RimBoost("Rim Light Boost", Float) = 2.0
    }

    Subshader
    {
        Pass
        {
            Tags { "RenderType"="Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NormalMap;
            float4 _NormalMap_ST;
            sampler2D _WarpTex;
            float _SpecExp;
            float _SpecBoost;
            float _RimExp;
            float _RimBoost;

            struct appData
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float3 tangent : TEXCOORD3;
                float3 binormal : TEXCOORD4;
            };

            v2f vert(appData v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.position);
                o.worldPos = mul(unity_ObjectToWorld, v.position);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.tangent = normalize(mul(unity_ObjectToWorld, v.tangent).xyz);
                o.binormal = normalize(cross(o.normal, o.tangent) * v.tangent.w * unity_WorldTransformParams.w);
                return o;
            }

            float4 frag(v2f i): SV_Target
            {
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 reflLightDir = reflect(-lightDir, i.normal);

                //切空间法向量转换
                float3 normalTangentSpace = UnpackNormal(tex2D(_NormalMap, i.uv.xy * _NormalMap_ST.xy + _NormalMap_ST.zw));
                float3x3 tangentToWorldSpace = float3x3(i.tangent, i.binormal, i.normal);
                float3 normal = mul(normalTangentSpace, tangentToWorldSpace);
                
                float4 albedo = tex2D(_MainTex, i.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw);

                //环境光
                float4 ambient = UNITY_LIGHTMODEL_AMBIENT;

                //漫反射
                float nDotL = dot(normal, lightDir);
                float4 ramp = tex2D(_WarpTex, float2(nDotL * 0.5 + 0.5, 0.5));
                float4 diffuse = _LightColor0 * ramp;

                //镜面反射
                float vDotR = DotClamped(viewDir, reflLightDir);
                float4 specular = _LightColor0 * _SpecBoost * pow(vDotR, _SpecExp);

                //Rim
                float fr = pow(1 - DotClamped(normal, viewDir), 4);
                float rim = fr * _RimBoost * pow(vDotR, _RimExp);

                //Dedicated Rim
                float4 drim = DotClamped(normal, float3(0, 1, 0)) * fr * _RimBoost;

                float4 lighting = ambient + diffuse + max(specular, rim) + drim;
                float4 final = albedo * lighting;
                return float4(final.rgb, 1.0);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}