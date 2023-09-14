Shader "Unlit/WaterCubeShader"
{
    Properties
    {
        _MainTex ("Texture", CUBE) = "" {}
        _EdgeWidth ("Width", range(0, 0.1)) = 0
        _DisplayMode ("Display Mode", int) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        Pass
        {
            ZWrite On
            ColorMask 0
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"

            samplerCUBE _MainTex;
            float _EdgeWidth;
            int _ShowEdge;

            struct appData
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 normal: NORMAL;
                float2 uv : TEXCOORD0;
                float3 local : TEXCOORD1;
            };

            v2f MyVertexProgram(appData v)
            {
                v2f i;
                i.position = UnityObjectToClipPos(v.position);
                i.normal = v.normal;
                i.local = v.position;
                i.uv = v.uv;
                return i;
            }

            float4 MyFragmentProgram(v2f i) : SV_Target
            {
                float4 edge = float4(0, 0, 0, 0);
                if (_EdgeWidth > 0.001f)
                    edge += saturate(step(i.uv.x, _EdgeWidth) + step(1 - _EdgeWidth, i.uv.x) + step(i.uv.y, _EdgeWidth) + step(1 - _EdgeWidth, i.uv.y));
                return edge;
            }
            ENDCG
        }
    }
}
