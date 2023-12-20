Shader "Custom/GrassMaterialBasedOnHeight"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _GrassMaterial ("Grass Material", Any) = "" { }
        _HeightThreshold ("Grass Height Threshold", Range (0, 1)) = 0.2
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input
        {
            float2 uv_MainTex;
            float height;
        };

        sampler2D _MainTex;
        fixed4 _Color;
        sampler2D _GrassMaterial;
        float _HeightThreshold;

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Texture principale
            o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color;

            // Condition basée sur la hauteur
            if (IN.height > _HeightThreshold)
            {
                // Appliquer le matériau d'herbe
                o.Albedo = tex2D (_GrassMaterial, IN.uv_MainTex).rgb;
            }
        }
        ENDCG
    }
    
    Fallback "Diffuse"
}
