Shader "Unlit/PlayerShader"
{
    Properties
    {
        _MainTex ("Texture", CUBE) = "" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"

            samplerCUBE _MainTex;

            struct appData
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 normal: NORMAL;
                float3 local : TEXCOORD0;
            };

            v2f MyVertexProgram(appData v)
            {
                v2f i;
                i.position = UnityObjectToClipPos(v.position);
                i.normal = v.normal;
                i.local = v.position;
                return i;
            }

            float4 MyFragmentProgram(v2f i) : SV_Target
            {
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;
                float3 myColor = texCUBE(_MainTex, normalize(i.local)).rgb;
                float3 diffuse = myColor * lightColor * DotClamped(lightDir, i.normal);
                fixed3 ambient = myColor * UNITY_LIGHTMODEL_AMBIENT.xyz;
                return float4(ambient + diffuse, 1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
