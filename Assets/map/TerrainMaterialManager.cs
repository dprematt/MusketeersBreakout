using UnityEngine;

[ExecuteInEditMode]
public class TerrainMaterialManager : MonoBehaviour
{
    public Material terrainMaterial; // Le matériau utilisant le shader personnalisé

    public Material waterMaterial;
    public Material sandMaterial;
    public Material grassMaterial;
    public Material mountainMaterial;

    void Start()
    {
        ApplyMaterials();
    }

    void ApplyMaterials()
    {
        if (terrainMaterial != null)
        {
            if (waterMaterial != null) terrainMaterial.SetTexture("_WaterTex", waterMaterial.mainTexture);
            if (sandMaterial != null) terrainMaterial.SetTexture("_SandTex", sandMaterial.mainTexture);
            if (grassMaterial != null) terrainMaterial.SetTexture("_GrassTex", grassMaterial.mainTexture);
            if (mountainMaterial != null) terrainMaterial.SetTexture("_MountainTex", mountainMaterial.mainTexture);
        }
    }

    void Update()
    {
        // Assurer que les matériaux sont appliqués en mode édition
        if (!Application.isPlaying)
        {
            ApplyMaterials();
        }
    }
}
