Shader "Unlit/PhongShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Shininess ("Shininess", float) = 10
    }
    SubShader
    {
        Pass
        {
            ZWrite on
            
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Shininess;

            struct appData
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 normal : TEXCOORD1;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD2;
            };

            v2f MyVertexProgram(appData v)
            {
                v2f i;
                i.position = UnityObjectToClipPos(v.position);
                i.normal = UnityObjectToWorldNormal(v.normal);
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                i.worldPos = mul(unity_ObjectToWorld, v.position);
                return i;
            }

            float4 MyFragmentProgram(v2f i) : SV_Target
            {
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;
                
                float3 texColor = tex2D(_MainTex, i.uv).rgb;
                
                float3 ambient = texColor * UNITY_LIGHTMODEL_AMBIENT.xyz;
                
                float3 diffuse = texColor * lightColor * DotClamped(i.normal, lightDir);
                
                float3 finalColor = ambient + diffuse;
    
                return float4(finalColor, 1);
            }
            
            ENDCG
        }
        
        Pass
        {
            ZWrite off
            Blend one one
            
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Shininess;

            struct appData
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 normal : TEXCOORD1;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD2;
            };

            v2f MyVertexProgram(appData v)
            {
                v2f i;
                i.position = UnityObjectToClipPos(v.position);
                i.normal = UnityObjectToWorldNormal(v.normal);
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                i.worldPos = mul(unity_ObjectToWorld, v.position);
                return i;
            }

            float4 MyFragmentProgram(v2f i) : SV_Target
            {
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;
                
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 halfwayDir = normalize(lightDir + viewDir);
                float3 specular = lightColor * pow(dot(i.normal, halfwayDir), _Shininess);
                
                float3 finalColor = specular;
    
                return float4(finalColor, 1);
            }
            
            ENDCG
        }
    }
}
