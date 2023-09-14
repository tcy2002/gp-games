#include "Lighting.cginc"
#include "UnityCG.cginc"
#include "AutoLight.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float4 pos : SV_POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
    float3 localPos : TEXCOORD1;
    float3 worldPos : TEXCOORD2;
    SHADOW_COORDS(3)
};

samplerCUBE _MainTex;
samplerCUBE _FurTex;

fixed _FurLength;
fixed _FurDensity;
fixed _FurThinness;
fixed _FurShading;

float4 _ForceGlobal;
float4 _ForcelocalPos;

fixed4 _RimColor;
half _RimPower;

float _EdgeWidth;
int _DisplayMode;

v2f vert_surface(appdata v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.localPos = v.vertex;
    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
    o.uv = v.uv;
    TRANSFER_SHADOW(o)
    return o;
}

v2f vert_base(appdata v)
{
    v2f o;
    float3 P = v.vertex.xyz + v.normal * _FurLength * FURSTEP;
    P += clamp(mul(unity_WorldToObject, _ForceGlobal).xyz + _ForcelocalPos.xyz, -1, 1) * pow(FURSTEP, 3) * _FurLength;
    o.pos = UnityObjectToClipPos(float4(P, 1.0));
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.localPos = v.vertex;
    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
    o.uv = v.uv;
    TRANSFER_SHADOW(o)
    return o;
}

fixed4 frag_surface(v2f i): SV_Target
{
    float3 lightDir = _WorldSpaceLightPos0.xyz;
    float3 lightColor = _LightColor0.rgb;
    float3 myColor = texCUBE(_MainTex, normalize(i.localPos)).rgb;
    float3 diffuse = myColor * lightColor * DotClamped(lightDir, i.normal);
    fixed3 ambient = myColor * UNITY_LIGHTMODEL_AMBIENT.xyz;

    float3 edge = float3(0, 0, 0);
    if (_EdgeWidth > 0.001f)
        edge += saturate(step(i.uv.x, _EdgeWidth) + step(1 - _EdgeWidth, i.uv.x) + step(i.uv.y, _EdgeWidth) + step(1 - _EdgeWidth, i.uv.y));

    UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)
    
    return float4(ambient + diffuse * atten + edge, 1);
}

fixed4 frag_base(v2f i): SV_Target
{
    if (_DisplayMode == 1)
    {
        return float4(0, 0, 0, 0);
    }
    
    float3 lightDir = _WorldSpaceLightPos0.xyz;
    float3 lightColor = _LightColor0.rgb;
    float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
    
    float3 myColor = texCUBE(_MainTex, normalize(i.localPos)).rgb;
    myColor -= pow(1 - FURSTEP, 3) * _FurShading;
    float rim = 1.0 - saturate(dot(viewDir, i.normal));
    myColor += fixed4(_RimColor.rgb * pow(rim, _RimPower), 1.0);
    
    float3 diffuse = myColor * lightColor * DotClamped(lightDir, i.normal);
    fixed3 ambient = myColor * UNITY_LIGHTMODEL_AMBIENT.xyz;
    
    fixed3 noise = texCUBE(_FurTex, normalize(i.localPos)).rgb;
    fixed alpha = clamp(noise - (FURSTEP * FURSTEP) * _FurDensity, 0, 1);

    UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)
    
    return fixed4(ambient + diffuse * atten, alpha);
}
