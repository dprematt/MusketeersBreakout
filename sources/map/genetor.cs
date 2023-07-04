using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generator : MonoBehaviour
{

    public int width;
    public int height;
    public float scale;

    public void SkeletonGenerator() {
        float[,] map = Skeleton.GenerateSkeleton(width, height, scale);

        DisplaySkeleton display = FindObjectOfType<DisplaySkeleton>();
        display.DrawSkeleton(map);
    }
}