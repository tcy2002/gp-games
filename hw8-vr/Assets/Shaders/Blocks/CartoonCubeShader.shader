Shader "Unlit/CartoonCubeShader"
{
    Properties
    {
        _MainTex("Main Texture", cube) = "white" {}
        _NormalMap("Normal Map", cube) = "bump" {}
        _RampTex("Warp Texture", 2D) = "white" {}
        
        _EdgeWidth ("Width", range(0, 0.1)) = 0
        _DisplayMode ("Display Mode", int) = 0
        
        _SpecExp("Specular Exponent", Range(0.1, 150)) = 20.0
        _SpecBoost("Specular Boost", Float) = 1.0
        
        _RimExp("Rim Light Exponent", Range(0.1, 150)) = 4.0
        _RimBoost("Rim Light Boost", Float) = 2.0
    }

    Subshader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma multi_compile_fwdbase 
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "UnityCG.cginc"

            samplerCUBE _MainTex;
            samplerCUBE _NormalMap;
            sampler2D _RampTex;
            float _SpecExp;
            float _SpecBoost;
            float _RimExp;
            float _RimBoost;
            float _EdgeWidth;

            struct appData
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

            v2f vert(appData v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.localPos = v.vertex;
                o.uv = v.uv;

                //将所有坐标转换到切线空间中进行计算
                TANGENT_SPACE_ROTATION;
                o.tangentNormal = mul(rotation, v.normal);
                o.tangentLightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
                o.tangentViewDir = mul(rotation, ObjSpaceViewDir(v.vertex));

                TRANSFER_SHADOW(o)
                return o;
            }

            float4 frag(v2f i): SV_Target
            {
                fixed3 tangentNormal = normalize(UnpackNormal(texCUBE(_NormalMap, normalize(i.localPos))));
                fixed3 tangentLightDir = normalize(i.tangentLightDir);
                fixed3 tangentViewDir = normalize(i.tangentViewDir);
                fixed3 reflTangentLightDir = normalize(reflect(-tangentNormal, tangentLightDir));

                //表面颜色
                float4 albedo = texCUBE(_MainTex, normalize(i.localPos));

                //环境光
                float4 ambient = UNITY_LIGHTMODEL_AMBIENT * albedo;

                //漫反射
                float nDotL = dot(tangentNormal, tangentLightDir);
                float4 ramp = tex2D(_RampTex, float2(nDotL * 0.5 + 0.5, 0.5));
                float4 diffuse = _LightColor0 * ramp;

                //镜面反射
                float vDotR = DotClamped(tangentViewDir, reflTangentLightDir);
                float4 specular = _LightColor0 * _SpecBoost * pow(vDotR, _SpecExp);

                //Rim
                float fr = pow(1 - DotClamped(tangentNormal, tangentViewDir), 4);
                float rim = fr * _RimBoost * pow(vDotR, _RimExp);

                //Dedicated Rim
                float4 drim = DotClamped(tangentNormal, float3(0, 1, 0)) * fr * _RimBoost;

                float3 edge = float3(0, 0, 0);
                if (_EdgeWidth > 0.001f)
                    edge += saturate(step(i.uv.x, _EdgeWidth) + step(1 - _EdgeWidth, i.uv.x) + step(i.uv.y, _EdgeWidth) + step(1 - _EdgeWidth, i.uv.y));

                float4 lighting = diffuse + max(specular, rim) + drim;
                float4 final = albedo * lighting;

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)
                
                return float4(ambient + final.rgb * atten + edge, 1.0);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}