Shader "Custom/TerrainShader" {
    Properties{
        _MainTex("Main Texture", 2D) = "white" { }
        _SecondTex("Second Texture", 2D) = "white" { }
        _ThirdTex("Third Texture", 2D) = "white" { }
        _HeightMultiplier("Height Multiplier", Float) = 1.0
    }

        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 100

            CGPROGRAM
            #pragma surface surf Lambert

            struct Input {
                float2 uv_MainTex;
                float2 uv_SecondTex;
                float2 uv_ThirdTex;
                float3 worldPos;
            };

            sampler2D _MainTex;
            sampler2D _SecondTex;
            sampler2D _ThirdTex;
            float _HeightMultiplier;

            void surf(Input IN, inout SurfaceOutput o) {
                float height = IN.worldPos.y * _HeightMultiplier;

                float threshold1 = 0.1;
                float threshold2 = 8;
                float threshold3 = 10;

                float blendFactor1 = saturate(height / threshold1);
                float blendFactor2 = saturate((height - threshold1) / (threshold2 - threshold1));
                float blendFactor3 = saturate((height - threshold2) / (threshold3 - threshold2));

                fixed3 col1 = tex2D(_MainTex, IN.uv_MainTex).rgb;
                fixed3 col2 = tex2D(_SecondTex, IN.uv_SecondTex).rgb;
                fixed3 col3 = tex2D(_ThirdTex, IN.uv_ThirdTex).rgb;

                fixed3 finalColor = lerp(lerp(col1, col2, blendFactor1), lerp(col2, col3, blendFactor2), blendFactor3);

                o.Albedo = finalColor;
                o.Alpha = 1.0;
            }
            ENDCG
        }

            FallBack "Diffuse"
}