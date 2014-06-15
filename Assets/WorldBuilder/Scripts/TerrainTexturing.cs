using System.Collections.Generic;
using UnityEngine;

public class TTexture
{
    public Texture2D texture { get; set; }
    public Color color { get; set; }
    public bool enableGrass { get; set; }
    public Texture2D bumpmap { get; set; }
    public bool useBump { get; set; }
    public Vector2 tilesize { get; set; }
    public int index { get; set; }
    public AnimationCurve heightCurve { get; set; }
    public AnimationCurve angleCurve { get; set; }
}

public class TerrainTexturing
{

    #region AdvancedTexturing

    //Advanced texturing
    public static void GenerateTexture(List<TTexture> textures)
    {
        Terrain t = Terrain.activeTerrain;
        TerrainData td = t.terrainData;

        SplatPrototype[] splatPrototypes = new SplatPrototype[textures.Count];

        for (int i = 0; i < textures.Count; i++)
        {
            splatPrototypes[i] = new SplatPrototype() { texture = (Texture2D)textures[i].texture, tileSize = textures[i].tilesize };
        }

        td.splatPrototypes = splatPrototypes;

        float[, ,] splatmaps = new float[td.alphamapWidth, td.alphamapHeight, td.alphamapLayers];

        float terrainMaxHeight = td.size.y;

        float x = 0.0f;
        while (x < td.alphamapHeight)
        {
            float y = 0.0f;
            while (y < td.alphamapWidth)
            {

                float height = td.GetHeight((int)x, (int)y);
                float heightScaled = height / terrainMaxHeight;

                float xS = x / td.heightmapWidth;
                float yS = y / td.heightmapHeight;

                float steepness = td.GetSteepness(xS, yS);
                float angleScaled = steepness / 90.0f;

                for (int i = 0; i < td.alphamapLayers; i++)
                {

                    switch (textures[i].index)
                    {
                        case(0):
                            if (i != 0)
                            {
                                splatmaps[(int)y, (int)x, i] = textures[i].heightCurve.Evaluate(heightScaled);
                                for (int hi = 0; hi < i; hi++)
                                {
                                    splatmaps[(int)y, (int)x, hi] *= (splatmaps[(int)y, (int)x, i] -1 )/ -1;
                                }
                            }
                            else
                            {
                                splatmaps[(int)y, (int)x, i] = textures[i].heightCurve.Evaluate(heightScaled);
                            }
                            break;
                        case(1):
                            splatmaps[(int)y, (int)x, i] = textures[i].angleCurve.Evaluate(angleScaled);
                            for (int ai = 0; ai < i; ai++)
                            {
                                splatmaps[(int)y, (int)x, ai] *= (splatmaps[(int)y, (int)x, i] -1 )/ -1;
                            }
                            break;
                        default:
                            break;
                    }

                    if (splatmaps[(int)y, (int)x, i] > 1.0f) { splatmaps[(int)y, (int)x, i] = 1.0f; }
                }
                y++;
            }
            x++;
        }

        

        //Bump mapping and overlay Colors
        for (int bi = 0; bi < td.alphamapLayers; bi++)
        {
            //Colors;
            Shader.SetGlobalColor("_Color" + bi, textures[bi].color);

            if (textures[bi].useBump)
            {
                if (textures[bi].bumpmap != null)
                {
                    Shader.SetGlobalTexture("_Bump" + bi, textures[bi].bumpmap);
                }
                else
                {
                    Texture2D tex = new Texture2D(td.alphamapWidth, td.alphamapHeight);
                    Shader.SetGlobalTexture("_Bump" + bi, tex);
                }
            }
            else
            {
                Texture2D tex = new Texture2D(td.alphamapWidth, td.alphamapHeight);
                Shader.SetGlobalTexture("_Bump" + bi, tex);
            }
        }

        td.SetAlphamaps(0, 0, splatmaps);

    }
    #endregion
}