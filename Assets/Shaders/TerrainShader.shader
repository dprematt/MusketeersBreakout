Shader "Custom/TerrainShaderWithMaterialReferences" {
    Properties {
        _WaterTex("Water Texture", 2D) = "white" { }
        _SandTex("Sand Texture", 2D) = "white" { }
        _GrassTex("Grass Texture", 2D) = "white" { }
        _MountainTex("Mountain Texture", 2D) = "white" { }
        _HeightMultiplier("Height Multiplier", Range(0.0, 10.0)) = 1.0
        _WaterHeight("Water Height", Range(0.0, 1.0)) = 0.1
        _SandHeight("Sand Height", Range(0.0, 1.0)) = 0.3
        _GrassHeight("Grass Height", Range(0.0, 1.0)) = 0.6
        _MountainHeight("Mountain Height", Range(0.0, 1.0)) = 1.0
    }

    SubShader {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        sampler2D _WaterTex;
        sampler2D _SandTex;
        sampler2D _GrassTex;
        sampler2D _MountainTex;
        float _HeightMultiplier;
        float _WaterHeight;
        float _SandHeight;
        float _GrassHeight;
        float _MountainHeight;

        void surf(Input IN, inout SurfaceOutputStandard o) {
            float height = IN.worldPos.y * _HeightMultiplier;

            fixed3 colWater = tex2D(_WaterTex, IN.uv_MainTex).rgb;
            fixed3 colSand = tex2D(_SandTex, IN.uv_MainTex).rgb;
            fixed3 colGrass = tex2D(_GrassTex, IN.uv_MainTex).rgb;
            fixed3 colMountain = tex2D(_MountainTex, IN.uv_MainTex).rgb;

            fixed3 finalColor;

            if (height < _WaterHeight * _HeightMultiplier) {
                finalColor = colWater;
            } else if (height < _SandHeight * _HeightMultiplier) {
                finalColor = colSand;
            } else if (height < _GrassHeight * _HeightMultiplier) {
                finalColor = colGrass;
            } else {
                finalColor = colMountain;
            }

            o.Albedo = finalColor;
            o.Alpha = 1.0;
        }
        ENDCG
    }

    Fallback "Standard"
}
