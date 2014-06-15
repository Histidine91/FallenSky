using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Displacement;
using CoherentNoise.Generation.Fractal;
using CoherentNoise.Generation.Modification;
using System.Collections.Generic;
using UnityEngine;

public class Heightmapbuilder : ThreadedJob 
{
    public float[,] Heightmap;
    public bool IsMoisturemap = false;

    public int TerrainSeed = 0;
    public Vector3 TerrainSize = Vector3.one;
    public int HeightmapResolution = 128;
    public Vector2 HeightmapScale = new Vector2(0.5f,0.5f);
    public float EdgeDir = 1f;
    public float Freq_mountain = 1f;

    public float BumpMultiplier = 0.0f;
    public float HeightMultiplier = 2.0f;
    public float Roughness = 1f;
    public float BumpRoughness = 1f;

    Generator noise_islands;
    Generator noise_mountain;
    Generator noise_ridge;
    Generator noise_bump;
    Generator noise_smallbumps;
    Generator noise_smallbumps2;

    int _seedcounter = 0;
    int _seed = 0;
    
    
    private int seed { get { return _seed + 1000 * _seedcounter++; } }

    protected override void ThreadFunction()
    {
        initNoiseGenerators();
        float scaleX = HeightmapScale.x;
        float scaleY = HeightmapScale.y;
        
        if (scaleX > 10f) scaleX *= (1f - scaleX*0.005f);
        if (scaleY > 10f) scaleY *= (1f - scaleY*0.005f);
        Vector3 pos;
        List<float> values = new List<float>();
        float heighestValue = 0f;
        for (int h = 0; h < Heightmap.GetLength(1); h++)
        {
            for (int w = 0; w < Heightmap.GetLength(0); w++)
            {
                pos = new Vector3(((w * scaleX)), 0, ((h * scaleY)));
                float bump = noise_bump.GetValue(pos);
                float continents = noise_islands.GetValue(pos);
                float mountain = noise_mountain.GetValue(pos);
                if (mountain >= 0.33f) { mountain -= 0.33f;  mountain *= noise_ridge.GetValue(pos); }
                else mountain = 0f;
                float smallbumps = noise_smallbumps.GetValue(pos);
                float smallbumps2 = noise_smallbumps2.GetValue(pos);
                
                float value = 0.05f + bump * bump * HeightMultiplier * 0.3f + (continents - 0.5f) * 0.1f + 0.75f * mountain + 0.009f * BumpMultiplier * smallbumps + 0.005f * smallbumps2;
                value = clipEdges(value, pos, h, w);
                Heightmap[h, w] = value;
                values.Add(value);
                if (value > heighestValue) heighestValue = value;
            }
        }
        if (heighestValue >= 1f)
        {
            for (int h = 0; h < Heightmap.GetLength(1); h++)
            {
                for (int w = 0; w < Heightmap.GetLength(0); w++)
                {
                    Heightmap[h, w] *= 1f / heighestValue;
                }
            }
        }
    }
    private float clipEdges(float value, Vector3 pos, int h, int w)
    {
        Vector2 center = new Vector2(0.5f * TerrainSize.x, 0.5f * TerrainSize.z);
        Vector2 pos2d = new Vector2(pos.x, pos.z);
        float dX = center.x - pos2d.x;
        dX = dX < 0 ? dX * -1 : dX;
        dX /= center.x;
        float dY = center.y - pos2d.y;
        dY = dY < 0 ? dY * -1 : dY;
        dY /= center.y;
        if (dX > dY) { value -= EdgeDir * Mathf.Pow(dX, 10f) * 0.75f; }
        else value -= EdgeDir * Mathf.Pow(dY, 10f) * 0.75f;
        return value;
    }

    private void initNoiseGenerators()
    {
        _seedcounter = 0;
        _seed = TerrainSeed;
        noise_islands =
            0.5f * (1f + new PinkNoise(seed) { Frequency = 0.001f * (Roughness), OctaveCount = 6, Persistence = 0.4f });

        noise_mountain = (0.5f * (1f + new Bias(new PinkNoise(seed) { Frequency = 0.000025f * Freq_mountain, OctaveCount = 2, Persistence = 0.5f }, 0f)));

        noise_ridge = new RidgeNoise(seed) { Frequency = 0.0005f };

        noise_bump = 0.5f * (1f + new Scale(new GradientNoise(seed), 0.0008f, 0, 0.0008f));

        noise_smallbumps = new PinkNoise(seed) { Frequency = 0.0075f, OctaveCount = 1 };
        noise_smallbumps2 = new RidgeNoise(seed) { Frequency = 0.0075f * BumpRoughness * 10f, OctaveCount = 4 };
    }
}
