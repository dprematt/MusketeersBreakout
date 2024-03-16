Shader "Custom/IsometricPixelArtShader"
{
    Properties
    {
        _EdgeColor("Edge Color", Color) = (1,1,1,1)
    }

        SubShader
    {
        Tags
        {
            "Queue" = "Overlay"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3

            // Propriétés du shader
            fixed4 _EdgeColor;

    // Fonction pour détecter les bords
    fixed IsEdge(float depth, float3 normal, float2 uv)
    {
        float thresholdDepth = 0.01; // Ajustez le seuil de profondeur selon vos besoins
        float thresholdNormal = 0.1; // Ajustez le seuil de normale selon vos besoins

        // Récupérer la profondeur et la normale du pixel voisin
        float neighborDepth = tex2D(_CameraDepthTexture, uv).r;
        float3 neighborNormal = tex2D(_CameraNormalsTexture, uv).xyz;

        // Comparer la différence de profondeur et de normales
        float depthDiff = abs(depth - neighborDepth);
        float normalDiff = length(normal - neighborNormal);

        // Retourner 1 si le pixel est un bord, sinon 0
        return (depthDiff > thresholdDepth || normalDiff > thresholdNormal) ? 1 : 0;
    }

    struct appdata
    {
        float4 pos : POSITION;
        float3 normal : NORMAL;
    };

    struct v2f
    {
        float4 pos : POSITION;
        float2 uv : TEXCOORD0;
    };

    v2f vert(appdata v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.pos);
        o.uv = ComputeGrabScreenPos(o.pos);
        return o;
    }

    fixed4 frag(v2f i) : COLOR
    {
        float depth = tex2D(_CameraDepthTexture, i.uv).r;
        float3 normal = tex2D(_CameraNormalsTexture, i.uv).xyz;

        // Détecter les bords en utilisant la fonction IsEdge
        fixed isEdge = IsEdge(depth, normal, i.uv);

        // Utiliser la couleur des bords ou la couleur d'origine
        fixed4 col = lerp(_EdgeColor, fixed4(1,1,1,1), isEdge);

        return col;
    }
    ENDCG
}
    }
}
