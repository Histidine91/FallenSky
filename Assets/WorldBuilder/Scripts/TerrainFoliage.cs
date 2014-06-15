using CoherentNoise;
using CoherentNoise.Generation.Combination;
using CoherentNoise.Generation.Fractal;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFoliage : MonoBehaviour {

    public static float waterLevel { get; set; }
    public static float maxSteepness { get; set; }
    public static int grassDensity { get; set; }

    public static void GenerateFoliage()
    {
        GenerateTrees();
    }

    private static void GenerateTrees()
    {
        Terrain t = Terrain.activeTerrain;
        TerrainData td = t.terrainData;

        TreePrototype[] treeprototypes = new TreePrototype[] { new TreePrototype() { prefab = (GameObject)Resources.Load("BigTree") }, new TreePrototype() { prefab = (GameObject)Resources.Load("Tree") } };

        td.treePrototypes = treeprototypes;

        //float[, ,] splatmaps = td.GetAlphamaps(0, 0, td.alphamapWidth, td.alphamapHeight);
        td.treeInstances = new TreeInstance[0];

        List<Vector3> treePos = new List<Vector3>();

        float[,] noisemap = new float[td.alphamapWidth, td.alphamapHeight];
        Generator noise_tree = new Max(
            new PinkNoise((int)UnityEngine.Random.Range(0, int.MaxValue)) { Frequency = 0.01f, OctaveCount = 6, Persistence = 0.66f, Lacunarity = 0.1f },
            new PinkNoise((int)UnityEngine.Random.Range(0, int.MaxValue)) { Frequency = 0.015f, OctaveCount = 2, Persistence = 0.66f, Lacunarity = 0.2f });
        for (int ny = 0; ny < noisemap.GetLength(1); ny++)
        {
            for (int nx = 0; nx < noisemap.GetLength(0); nx++)
            {
                noisemap[nx, ny] = noise_tree.GetValue(nx, ny, 0);
            }
        }
        if (maxSteepness == 0) { maxSteepness = 70.0f; }
        if (waterLevel == 0) { waterLevel = 0.0f; }
        float x = 0.0f;
        while (x < td.alphamapWidth)
        {
            float y = 0.0f;
            while (y < td.alphamapHeight)
            {
                float height = td.GetHeight((int)x, (int)y);
                float heightScaled = height / td.size.y;
                float xScaled = (x + Random.Range(-1f, 1f)) / td.alphamapWidth;
                float yScaled = (y + Random.Range(-1f, 1f)) / td.alphamapHeight;
                float steepness = td.GetSteepness(xScaled, yScaled);

                if (Random.Range(0f, 1f) > 1f - noisemap[(int)x, (int)y] * 2f && steepness < maxSteepness && height > waterLevel)
                {
                    treePos.Add(new Vector3(xScaled, heightScaled, yScaled));
                }

                y++;
            }
            x++;
        }

        TreeInstance[] treeInstances = new TreeInstance[treePos.Count];

        for (int ii = 0; ii < treeInstances.Length; ii++)
        {
            treeInstances[ii].position = treePos[ii];
            treeInstances[ii].prototypeIndex = Random.Range(0, treeprototypes.Length);
            treeInstances[ii].color = Color.white;//new Color(Random.Range(200, 255), Random.Range(200, 255), Random.Range(200, 255));
            treeInstances[ii].lightmapColor = Color.white;
            treeInstances[ii].heightScale = 1.0f + Random.Range(-0.25f, 0.5f);
            treeInstances[ii].widthScale = 1.0f + Random.Range(-0.5f, 0.25f);
        }
        td.treeInstances = treeInstances;
    }

    public static void ClearTrees()
    {
        Terrain t = Terrain.activeTerrain;
        TerrainData td = t.terrainData;
        TreePrototype[] treeprototypes = new TreePrototype[0];
        td.treePrototypes = treeprototypes;
        
    }

    public static void GenerateGrass()
    {
        Terrain t = Terrain.activeTerrain;
        TerrainData td = t.terrainData;

        if (grassDensity == 0)
        {
            grassDensity = 5;
        }

        DetailPrototype[] detailPrototypes = new DetailPrototype[2];

        detailPrototypes[0] = new DetailPrototype() { prototypeTexture = Resources.Load("Grass") as Texture2D };
        detailPrototypes[1] = new DetailPrototype() { prototypeTexture = Resources.Load("Grass2") as Texture2D };

        td.detailPrototypes = detailPrototypes;

        for (int i = 0; i < td.detailPrototypes.Length; i++)
        {
            int[,] detailLayer = td.GetDetailLayer(0, 0, td.detailWidth, td.detailHeight, i);

            float x = 0.0f;
            while (x < td.detailWidth)
            {
                float y = 0.0f;
                while (y < td.detailHeight)
                {
                    detailLayer[(int)x, (int)y] = 10;
                    y++;
                }
                x++;
            }

            td.SetDetailLayer(0, 0, i, detailLayer);
        }   
    }

    public static void ClearGrass()
    {
        Terrain t = Terrain.activeTerrain;
        TerrainData td = t.terrainData;
        DetailPrototype[] detailPrototypes = new DetailPrototype[0];
        td.detailPrototypes = detailPrototypes;
    }
}
