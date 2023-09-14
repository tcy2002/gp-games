Shader "Unlit/NormalShader"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"

            float4 _MainColor;

            struct appData
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 normal : TEXCOORD0;
            };

            v2f MyVertexProgram(appData v)
            {
                v2f i;
                i.position = UnityObjectToClipPos(v.position);
                i.normal = UnityObjectToWorldNormal(v.normal);
                return i;
            }

            float4 MyFragmentProgram(v2f i) : SV_Target
            {
                return float4(i.normal, 1);
            }
            
            ENDCG
        }
    }
}
