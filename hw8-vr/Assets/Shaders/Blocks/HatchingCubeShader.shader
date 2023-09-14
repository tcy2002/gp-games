Shader "Unlit/HatchingCubeShader" {
	Properties {
		_MainTex("Main Texture", cube) = "white" {}
        _NormalMap("Normal Map", cube) = "bump" {}
        _RampTex("Warp Texture", 2D) = "white" {}
		
		_EdgeWidth("Width", range(0, 0.1)) = 0
        _DisplayMode("Display Mode", int) = 0
		
		_HatchFactor ("Hatch Factor", Float) = 1
		_Hatch0 ("Hatch 0", CUBE) = "white" {}
		_Hatch1 ("Hatch 1", CUBE) = "white" {}
		_Hatch2 ("Hatch 2", CUBE) = "white" {}
		_Hatch3 ("Hatch 3", CUBE) = "white" {}
		_Hatch4 ("Hatch 4", CUBE) = "white" {}
		_Hatch5 ("Hatch 5", CUBE) = "white" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		Pass {
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag 
			
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityShaderVariables.cginc"
			
			samplerCUBE _MainTex;
			samplerCUBE _Hatch0;
			samplerCUBE _Hatch1;
			samplerCUBE _Hatch2;
			samplerCUBE _Hatch3;
			samplerCUBE _Hatch4;
			samplerCUBE _Hatch5;
			float _HatchFactor;
			float _EdgeWidth;
			
			struct a2v {
				float4 vertex : POSITION;
				float4 tangent : TANGENT; 
				float3 normal : NORMAL; 
				float2 uv : TEXCOORD0; 
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float3 localPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				SHADOW_COORDS(4)
			};
			
			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.localPos = v.vertex;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.uv = v.uv;
				TRANSFER_SHADOW(o);
				return o; 
			}
			
			fixed4 frag(v2f i) : SV_Target {
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				fixed lDotN = DotClamped(lightDir, i.worldNormal);

				float ambient = UNITY_LIGHTMODEL_AMBIENT.x;
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)

				float hatchFactor = lDotN * 7.0 * _HatchFactor * (atten + ambient);

				float w1 = 0, w2 = 0, w3 = 0, w4 = 0, w5 = 0, w6 = 0;
				if (hatchFactor > 6.0) {
					//无阴影
				} else if (hatchFactor > 5.0) {
					w1 = hatchFactor - 5.0;
				} else if (hatchFactor > 4.0) {
					w1 = hatchFactor - 4.0;
					w2 = 1.0 - w1;
				} else if (hatchFactor > 3.0) {
					w2 = hatchFactor - 3.0;
					w3 = 1.0 - w2;
				} else if (hatchFactor > 2.0) {
					w3 = hatchFactor - 2.0;
					w4 = 1.0 - w3;
				} else if (hatchFactor > 1.0) {
					w4 = hatchFactor - 1.0;
					w5 = 1.0 - w4;
				} else {
					w5 = hatchFactor;
					w6 = 1.0 - w5;
				}
				
				fixed3 localPos = normalize(i.localPos);
				fixed4 hatchTex0 = texCUBE(_Hatch0, localPos) * w1;
				fixed4 hatchTex1 = texCUBE(_Hatch1, localPos) * w2;
				fixed4 hatchTex2 = texCUBE(_Hatch2, localPos) * w3;
				fixed4 hatchTex3 = texCUBE(_Hatch3, localPos) * w4;
				fixed4 hatchTex4 = texCUBE(_Hatch4, localPos) * w5;
				fixed4 hatchTex5 = texCUBE(_Hatch5, localPos) * w6;
				fixed4 whiteColor = fixed4(1, 1, 1, 1) * (1 - w1 - w2 - w3 - w4 - w5 - w6);
				
				fixed4 hatchColor = hatchTex0 + hatchTex1 + hatchTex2 + hatchTex3 + hatchTex4 + hatchTex5 + whiteColor;
				
				float3 albedo = texCUBE(_MainTex, normalize(i.localPos));
				float gray = albedo.x * 0.299f + albedo.y * 0.587f + albedo.z * 0.114f;
				float3 reverseHatch = 1 - hatchColor.rgb;
				float3 reverseColor = reverseHatch * (1 - gray);
				float3 color = 1 - reverseColor;

				float3 edge = float3(1, 1, 1);
                if (_EdgeWidth > 0.001f)
                    edge -= saturate(step(i.uv.x, _EdgeWidth) + step(1 - _EdgeWidth, i.uv.x) + step(i.uv.y, _EdgeWidth) + step(1 - _EdgeWidth, i.uv.y));
				
				return fixed4(color * edge, 1.0);
			}
			
			ENDCG
		}
	} 
	
	FallBack "Diffuse"
}