Shader "Custom/TerrainShader" {
    Properties {
        _MainTex("Main Texture", 2D) = "white" { }
        _SecondTex("Second Texture", 2D) = "white" { }
        _ThirdTex("Third Texture", 2D) = "white" { }
        _FourthTex("Fourth Texture", 2D) = "white" { } // Nouvelle texture
        _HeightMultiplier("Height Multiplier", Float) = 1.0
        _Threshold1("Threshold 1", Float) = 0.1 // Nouveau seuil
        _Threshold2("Threshold 2", Float) = 0.2 // Nouveau seuil
        _Threshold3("Threshold 3", Float) = 0.5 // Nouveau seuil
        _Threshold4("Threshold 4", Float) = 0.8 // Nouveau seuil
    }

    SubShader {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input {
            float2 uv_MainTex;
            float2 uv_SecondTex;
            float2 uv_ThirdTex;
            float2 uv_FourthTex; // Nouvelles coordonnées de texture
            float3 worldPos;
        };

        sampler2D _MainTex;
        sampler2D _SecondTex;
        sampler2D _ThirdTex;
        sampler2D _FourthTex; // Nouvelle texture
        float _HeightMultiplier;
        float _Threshold1; // Nouveau seuil
        float _Threshold2; // Nouveau seuil
        float _Threshold3; // Nouveau seuil
        float _Threshold4; // Nouveau seuil

        void surf(Input IN, inout SurfaceOutput o) {
            float height = IN.worldPos.y * _HeightMultiplier;

        float threshold1 = _Threshold1; 
        float threshold2 = _Threshold2;
        float threshold3 = _Threshold3;
        float threshold4 = _Threshold4;

        float blendFactor1 = step(height, threshold2);
        float blendFactor2 = step(height, threshold3) - blendFactor1;
        float blendFactor3 = step(height, threshold4) - blendFactor1 - blendFactor2;

        fixed3 col1 = tex2D(_MainTex, IN.uv_MainTex).rgb;
        fixed3 col2 = tex2D(_SecondTex, IN.uv_SecondTex).rgb;
        fixed3 col3 = tex2D(_ThirdTex, IN.uv_ThirdTex).rgb;
        fixed3 col4 = tex2D(_FourthTex, IN.uv_FourthTex).rgb;

        fixed3 finalColor = col1 * blendFactor1 + col2 * blendFactor2 + col3 * blendFactor3 + col4 * (1 - blendFactor1 - blendFactor2 - blendFactor3);
        o.Albedo = finalColor;
        //o.Albedo = step(IN.worldPos.y, 0).xxx;
        //o.Albedo = ((IN.worldPos.y >= 0)?1:0).xxx
        o.Alpha = 1.0;
    }
        ENDCG
    }

    Fallback "Diffuse"
}
