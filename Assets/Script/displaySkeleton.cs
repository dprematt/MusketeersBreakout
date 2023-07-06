using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySkeleton : MonoBehaviour
{
    public Renderer textureRenderer;
     
    public void DrawSkeleton(float[,] skeleton) {
        int width = skeleton.GetLength(0);
        int height = skeleton.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colorMap = new Color[width * height];

        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                if (j % 16 == 0 && i % 16 == 0) {
                    colorMap[i * width + j] = Color.Lerp(Color.red, Color.red, skeleton[j, i]);
                } else {
                    colorMap[i * width + j] = Color.Lerp(Color.black, Color.white, skeleton[j, i]);
                }
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(width, 1, height);
 
    }
}
