using UnityEngine;

public class TerrainGeneration
{
    public bool SurroundedByMountains = false;
    public string TerrainSeed = null;
    public static TerrainGeneration current;
    public static float Waterlevel { get { return current.waterplane.transform.position.y; } }
    public GameObject waterplane;
    public float SetMountainFreq = 1f;
    public float SetWaterlevel = 0.1f;
    public float BumpMultiplier = 1f;
    public float HeightMultiplier = 1f;
    public float Roughness = 1f;
    public float BumbRoughness = 1f;
    public bool editor = false;

    public Terrain terrain;

    #region Private variables
    
    Heightmapbuilder terrainbuilder;
    
    #endregion

    private int _seed = 0;
    public int Seed { get { return _seed; } }

    public void makeHeightmap() 
    {
        int index = 1;
        foreach (char c in TerrainSeed) { _seed += (index++ * (int)c); }

        TerrainData tdata = terrain.terrainData;
        terrainbuilder = new Heightmapbuilder()
        {
            Heightmap = new float[(int)(tdata.heightmapWidth), (int)(tdata.heightmapHeight)],
            TerrainSize = tdata.size,
            TerrainSeed = Seed,
            HeightmapScale = new Vector2(tdata.heightmapScale.x, tdata.heightmapScale.z),
            EdgeDir = SurroundedByMountains ? -1f : 1f,
            Freq_mountain = this.SetMountainFreq,
            HeightMultiplier = this.HeightMultiplier,
            Roughness = this.Roughness,
            BumpMultiplier = this.BumpMultiplier,
            BumpRoughness = this.BumbRoughness,
        };
        terrainbuilder.Start();

        //For editor calls
        if (editor)
        {
            while (!terrainbuilder.Update()) ;
            terrain.terrainData.SetHeights(0, 0, terrainbuilder.Heightmap);
            editor = false;
        }
    }

    //You can use this to get Random position on terrain
    public Vector3 RandomPositionOnLand()
    {
        bool land = false;
        Vector3 pos = Vector3.zero;
        while (!land)
        {
            pos = new Vector3(Random.Range(0f, terrain.terrainData.size.x), 0, Random.Range(0f, terrain.terrainData.size.z));
            float height = terrain.SampleHeight(pos);
            land =  height > Waterlevel && height < terrain.terrainData.size.y*0.5f;
        }
        return pos;
    }

}