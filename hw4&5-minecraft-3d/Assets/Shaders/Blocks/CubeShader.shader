Shader "Unlit/CubeShader"
{
    Properties
    {
        _MainTex("Main Texture", cube) = "white" {}
        _NormalMap("Normal Map", cube) = "bump" {}
        _RampTex("Warp Texture", 2D) = "white" {}
        
        _EdgeWidth ("Width", range(0, 0.1)) = 0
        _DisplayMode ("Display Mode", int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma multi_compile_fwdbase
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            samplerCUBE _MainTex;
            samplerCUBE _NormalMap;
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

            v2f MyVertexProgram(appData v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.localPos = normalize(v.vertex);
                o.uv = v.uv;
                
                //将所有坐标转换到切线空间中进行计算
                TANGENT_SPACE_ROTATION;
                o.tangentNormal = mul(rotation, v.normal);
                o.tangentLightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
                o.tangentViewDir = mul(rotation, ObjSpaceViewDir(v.vertex));

                TRANSFER_SHADOW(o)
                return o;
            }

            float4 MyFragmentProgram(v2f i) : SV_Target
            {
                fixed3 tangentNormal = normalize(UnpackNormal(texCUBE(_NormalMap, normalize(i.localPos))));
                fixed3 tangentLightDir = normalize(i.tangentLightDir);
                
                float3 albedo = texCUBE(_MainTex, normalize(i.localPos)).rgb;
                float3 diffuse = albedo * _LightColor0 * DotClamped(tangentLightDir, tangentNormal);
                fixed3 ambient = albedo * UNITY_LIGHTMODEL_AMBIENT.xyz;
                
                float3 edge = float3(0, 0, 0);
                if (_EdgeWidth > 0.001f)
                    edge += saturate(step(i.uv.x, _EdgeWidth) + step(1 - _EdgeWidth, i.uv.x) + step(i.uv.y, _EdgeWidth) + step(1 - _EdgeWidth, i.uv.y));

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)
                
                return float4(ambient + diffuse * atten + edge, 1);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
