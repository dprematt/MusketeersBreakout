public struct MapData
{
    public readonly float[,] heightMap;
    public readonly float[,] colorMap;

    public MapData(float[,] heightMap, float[,] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}