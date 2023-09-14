Shader "Unlit/FurCubeShader"
{
    Properties
    {
        _MainTex ("Texture", cube) = "white" { }
        _FurTex ("Fur Pattern", cube) = "white" { }
        
        _FurLength ("Fur Length", Range(0.0, 1)) = 0.1
        _FurDensity ("Fur Density", Range(0, 2)) = 1
        _FurThinness ("Fur Thinness", Range(0.01, 10)) = 5
        _FurShading ("Fur Shading", Range(0.0, 1)) = 0.5

        _ForceGlobal ("Force Global", Vector) = (0, 0, 0, 0)
        _ForceLocal ("Force Local", Vector) = (0, 0, 0, 0)
        
        _RimColor ("Rim Color", Color) = (0, 0, 0, 1)
        _RimPower ("Rim Power", Range(0.0, 8.0)) = 6.0
        
        _EdgeWidth ("Edge Width", range(0, 0.1)) = 0.01
        _DisplayMode ("Display Mode", int) = 0
    }
    
    Category
    {
        Tags { "RenderType"="Transparent"}
        Cull Off
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha
        
        SubShader
        {
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_surface
                #pragma fragment frag_surface
                #define FURSTEP 0.00
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }

            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.05
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.10
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.15
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.20
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.25
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.30
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.35
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.40
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.45
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.50
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.55
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.60
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.65
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.70
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.75
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.80
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.85
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.90
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.95
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }

            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 1.00
                #include "./FurCubeHelper.cginc"
                
                ENDCG
                
            }
        }
    }
    FallBack "Diffuse"
}