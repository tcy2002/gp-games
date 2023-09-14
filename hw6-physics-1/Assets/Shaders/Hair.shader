Shader "Unlit/Hair"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_MainColor("Hair Color(头发颜色)", Color) = (0,0,0,0)
		_SpecularShift("Hair Shifted Texture(头发渐变灰度图)", 2D) = "white" {}
		_SpecularColor("Hair Spec Color Primary(主高光颜色)", Color) = (1,1,1,1)
		_PrimaryShift("Primary Shift(主高光偏移)", Range(-5, 5)) = 0
		_SpecularWidth("Spec Width(高光宽度)", Range(0, 1)) = 1
		_SpecularScale("Spec Scale(高光强度)", Range(0, 2)) = 1
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent :TANGENT;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				float3 bitangent : TEXCOORD4;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainColor;
			float4 _SpecularColor;
			sampler2D _SpecularShift;
			float4 _SpecularShift_ST;
			fixed _PrimaryShift;
			float _SpecularWidth;
			fixed _SpecularScale;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv = float2(o.uv.y, o.uv.x);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = mul(unity_ObjectToWorld, v.normal);
				o.bitangent = mul(unity_ObjectToWorld, cross(v.normal, v.tangent));
				return o;
			}

			half3 ShiftedTangent(float3 t, float3 n, float shift) {
				return normalize(t + shift * n);
			}

			float StrandSpecular(float3 T, float3 V, float3 L, int exponent)
			{
				float3 H = normalize(L + V);
				float dotTH = -abs(dot(T, H));
				float sinTH = sqrt(1.0 - dotTH * dotTH);
				float dirAtten = smoothstep(-_SpecularWidth, 0, dotTH);
				return dirAtten * pow(sinTH, exponent) * _SpecularScale;
			}

			float3 HairSpecular(float3 t, float3 n, float3 l, float3 v, float2 uv)
			{
				float shiftTex = tex2D(_SpecularShift, uv) - 0.5;
				float3 t1 = ShiftedTangent(t, n, _PrimaryShift + shiftTex);
				return _SpecularColor * StrandSpecular(t1, v, l, 20);
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				//diffuse
				fixed3 diffuse = tex2D(_MainTex, i.uv).rgb * _MainColor;
				//specular
				float3 normal = normalize(i.worldNormal);
				float3 bitangent = normalize(i.bitangent);
				float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				float3 speculer = HairSpecular(bitangent, normal, lightDir, viewDir, i.uv);
				return float4(_LightColor0 * (speculer + diffuse), 1);
			}
			ENDCG
		}
	}
}