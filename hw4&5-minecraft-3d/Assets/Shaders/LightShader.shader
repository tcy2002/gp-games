Shader "Unlit/LightShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appData
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            v2f MyVertexProgram(appData v)
            {
                v2f i;
                i.position = UnityObjectToClipPos(v.position);
                i.normal = UnityObjectToWorldNormal(v.normal);
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return i;
            }

            float4 MyFragmentProgram(v2f i) : SV_Target
            {
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;
                float3 myColor = tex2D(_MainTex, i.uv).rgb;
                float3 diffuse = myColor * lightColor * DotClamped(lightDir, i.normal);
                fixed3 ambient = myColor * UNITY_LIGHTMODEL_AMBIENT.xyz;
                return float4(ambient + diffuse, 1);
            }
            
            ENDCG
        }
    }
}
