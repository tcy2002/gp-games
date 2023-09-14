Shader "Post/DepthBlurShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _BlurTex ("Blur Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 1.0
        _Ramp("Ramp", Float) = 10
    }
    
    SubShader
    {
        CGINCLUDE

        #include "UnityCG.cginc"

        sampler2D _MainTex;
        half4 _MainTex_TexelSize;
        sampler2D _BlurTex;
        sampler2D _CameraDepthTexture;
        float _BlurSize;
        float _Ramp;

        struct v2f
        {
            float4 pos : SV_POSITION;
            half4 uv : TEXCOORD0;
            half2 uv_depth : TEXCOORD1;
        };

        struct v2f_g
        {
            float4 pos : SV_POSITION;
            half2 uv[5] : TEXCOORD0;
        };

        v2f vert(appdata_img v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv.xy = v.texcoord;
            o.uv.zw = v.texcoord;
            o.uv_depth = v.texcoord;
            
            #if UNITY_UV_STARTS_AT_TOP
            if(_MainTex_TexelSize.y < 0) {
                o.uv.w = 1 - o.uv.w;
                o.uv_depth.y = 1 - o.uv_depth.y;
            }
            #endif

            return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
            fixed4 col = tex2D(_MainTex, i.uv.xy);
            fixed4 bcol = tex2D(_BlurTex, i.uv.zw);
            
            float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv_depth));
            float ramp = smoothstep(0, _Ramp, depth);
            
            return lerp(col, bcol, ramp);
        }

        v2f_g vert_g_v(appdata_img v)
        {
            v2f_g o;
            o.pos = UnityObjectToClipPos(v.vertex);
            //以扩散的方式对数组进行排序，只偏移y轴，其中1和2,3和4分别位于原始点0的上下，且距离1个单位和2个像素单位
            //得到的最终偏移与模糊范围的控制参数进行乘积
            half2 uv = v.texcoord;
            o.uv[0] = uv;
            o.uv[1] = uv + float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
            o.uv[2] = uv - float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
            o.uv[3] = uv + float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
            o.uv[4] = uv - float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
            return o;
        }        
        
        //用于计算横向模糊的纹理坐标元素
        v2f_g vert_g_h(appdata_img v)
        {
            v2f_g o;
            o.pos = UnityObjectToClipPos(v.vertex);
            //与上面同理，只不过是x轴向的模糊偏移
            half2 uv = v.texcoord;
            o.uv[0] = uv;
            o.uv[1] = uv + float2( _MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
            o.uv[2] = uv - float2( _MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
            o.uv[3] = uv + float2( _MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
            o.uv[4] = uv - float2( _MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
            return o;
        }
        
        //在片元着色器中进行最终的模糊计算，此过程在每个Pass中都会进行一次计算，但计算方式是统一的
        fixed4 frag_g(v2f_g i) : SV_Target
        {
            float weights[3] = {0.4026, 0.2442, 0.0545};
            fixed3 sum = tex2D(_MainTex, i.uv[0]).rgb * weights[0];
            //对采样结果进行对应纹理偏移坐标的权重计算，以得到模糊的效果
            for (int it = 1; it < 3; it++) 
            {
                sum += tex2D(_MainTex, i.uv[2 * it - 1]).rgb * weights[it];//上方两像素
                sum += tex2D(_MainTex, i.uv[2 * it]).rgb * weights[it];//下方两像素
            }
            return fixed4(sum, 1.0);
        }

        ENDCG

        ZTest Always Cull Off ZWrite Off

        //前两个Pass高斯模糊
        Pass
        {
            CGPROGRAM

            #pragma vertex vert_g_v
            #pragma fragment frag_g

            ENDCG
        }

        Pass
        {
            CGPROGRAM

            #pragma vertex vert_g_h
            #pragma fragment frag_g

            ENDCG
        }
        
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            ENDCG
        }
    }
    FallBack Off
}
