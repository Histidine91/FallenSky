using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Generation
{
    //Terrain
    [SerializeField]
    public float MountainFreq = 25.0f;
    [SerializeField]
    public float BumpbMultiplier = 1f;
    [SerializeField]
    public float HeightMultiplier = 1f;
    [SerializeField]
    public float Roughness = 2.5f;
    [SerializeField]
    public float BumpRoughness = 0f;
    [SerializeField]
    public string seed = "null";
    [SerializeField]
    public bool randomSeed = true;

    //Water
    //[SerializeField]
    //private bool imported = false;
    [SerializeField]
    public bool useWater = false;
    [SerializeField]
    public float WaterLevel = 0.1f;


    public void OnGUI()
    {
        MountainFreq = EditorGUILayout.Slider("Mountain Frequency", MountainFreq, 0, 100);
        Roughness = EditorGUILayout.Slider("Roughness", Roughness, 0, 5);
        HeightMultiplier = EditorGUILayout.Slider("Height multiplier", HeightMultiplier, 0, 3);
        BumpbMultiplier = EditorGUILayout.Slider("Bump multiplier", BumpbMultiplier, 0, 3);
        BumpRoughness = EditorGUILayout.Slider("Bump roughness", BumpRoughness, 0, 2);

        randomSeed = EditorGUILayout.Toggle("Use random seed", randomSeed);
        if (!randomSeed)
        {
            EditorGUILayout.HelpBox("abc123", MessageType.Info);
            seed = EditorGUILayout.TextField("Seed", seed);
        }

        useWater = EditorGUILayout.Toggle("Use water", useWater);
        if (useWater)
        {
            if (GameObject.Find("Water"))
            {
                GameObject water = GameObject.Find("Water");
                water.transform.position = new Vector3(water.transform.position.x, WaterLevel, water.transform.position.z);
                WaterLevel = EditorGUILayout.FloatField("Waterlevel", WaterLevel);
            }
            else
            {
                EditorGUILayout.HelpBox("No water found!\n Name your water GameObject as 'Water'", MessageType.Error); 
            }
        }

        if(GUILayout.Button("Generate")){
            TerrainGeneration TG = new TerrainGeneration();
            TG.SetMountainFreq = MountainFreq;
            TG.SetWaterlevel = WaterLevel;
            TG.BumpMultiplier = BumpbMultiplier;
            TG.BumbRoughness = BumpRoughness;
            TG.HeightMultiplier = HeightMultiplier;
            TG.Roughness = Roughness;
            TG.terrain = Terrain.activeTerrain;
            if (useWater && GameObject.Find("Water")) { TG.waterplane = GameObject.Find("Water");  TerrainFoliage.waterLevel = WaterLevel; }
            TG.editor = true;
            if (randomSeed)
            {
                TG.TerrainSeed = "" + (int)UnityEngine.Random.Range(0, int.MaxValue);
                seed = TG.TerrainSeed;
            } else
                TG.TerrainSeed = seed;
            Debug.Log("Using seed: " + TG.TerrainSeed);
            TG.makeHeightmap();
        }

        Terrain.activeTerrain.heightmapPixelError = EditorGUILayout.Slider("Pixel error", Terrain.activeTerrain.heightmapPixelError, 1, 200);
        Terrain.activeTerrain.basemapDistance = EditorGUILayout.Slider("Basemap distance", Terrain.activeTerrain.basemapDistance, 0, 2000);
    }
}

[Serializable]
public class wbTexturing
{
    [SerializeField]
    public Texture2D texture;
    [SerializeField]
    public bool useBump = false;
    [SerializeField]
    public Texture2D bumpmap;
    [SerializeField]
    public Vector2 tilesize = new Vector2(50, 50);
    [SerializeField]
    public bool enableGrass = false;


    public Texture2D emptyBump;

    [SerializeField]
    public Color color = Color.white;

    [SerializeField]
    public AnimationCurve heightCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);

    [SerializeField]
    public AnimationCurve angleCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [SerializeField]
    private string[] options = { "Height", "Angle" };
    [SerializeField]
    public int index = 0;



    public void OnGUI()
    {
        texture = EditorGUILayout.ObjectField("Texture", texture, typeof(Texture2D)) as Texture2D;
        tilesize = EditorGUILayout.Vector2Field("Tilesize", tilesize);

        index = EditorGUILayout.Popup(index, options);
        switch (index)
        {
            case (0):
                heightCurve = EditorGUILayout.CurveField("Height Curve", heightCurve);
                break;
            case (1):
                angleCurve = EditorGUILayout.CurveField("Angle Curve", angleCurve);
                break;
            default:
                break;
        }

        EditorGUILayout.LabelField("");
    }
}


[Serializable]
public class WorldGen : ScriptableObject{

    [SerializeField]
    private Generation genClass;

    [SerializeField]
    private List<wbTexturing> texClass;

    [SerializeField]
    private bool useTexturing = false;

    [SerializeField]
    private bool useFoliage = false;

    [SerializeField]
    private float waterCoast = 0.0f;

    [SerializeField]
    private float maxSteepness = 45.0f;

    [SerializeField]
    private int grassDensity = 5;

    

    public void OnEnable()
    {
        hideFlags = HideFlags.HideAndDontSave;
        if (genClass == null)
        {
            genClass = new Generation();
        }
        if (texClass == null)
        {
            texClass = new List<wbTexturing>();
        }
    }

    public void OnGUI()
    {
        genClass.OnGUI();

        useTexturing = EditorGUILayout.BeginToggleGroup("Setup textures", useTexturing);

        if (useTexturing)
        {
            if (GUILayout.Button("Clear all textures"))
            {
                List<TTexture> textures = new List<TTexture>();
                TerrainTexturing.GenerateTexture(textures);
            }

            foreach (var texture in texClass)
            {
                texture.OnGUI();
            }

            if (texClass.Count > 0)
            {
                if (GUILayout.Button("Delete last"))
                {
                    texClass.RemoveAt(texClass.Count - 1);
                }
            }

            if (GUILayout.Button("Add texture"))
            {
                texClass.Add(new wbTexturing());
            }

            if (texClass.Count > 0 && texClass[texClass.Count - 1].texture != null)
            {
                if (GUILayout.Button("Assign new textures"))
                {
                    List<TTexture> textures = new List<TTexture>();
                    for (int i = 0; i < texClass.Count; i++)
                    {
                        TTexture TTex = new TTexture();
                        TTex.texture = texClass[i].texture;
                        //TTex.color = texClass[i].color;
                        TTex.useBump = texClass[i].useBump;
                        if (texClass[i].useBump)
                        {
                            TTex.bumpmap = texClass[i].bumpmap;
                        }
                        else
                        {
                            TTex.bumpmap = texClass[i].emptyBump;
                        }
                        TTex.tilesize = texClass[i].tilesize;
                        TTex.index = texClass[i].index;
                        TTex.heightCurve = texClass[i].heightCurve;
                        TTex.angleCurve = texClass[i].angleCurve;
                        textures.Add(TTex);
                    }
                    TerrainTexturing.GenerateTexture(textures);
                }
            }
        }
        EditorGUILayout.EndToggleGroup();

        useFoliage = EditorGUILayout.BeginToggleGroup("Setup foliage", useFoliage);

        if (useFoliage)
        {
            waterCoast = EditorGUILayout.FloatField("Tree distance from coast", waterCoast);
            maxSteepness = EditorGUILayout.FloatField("Max hill angle for trees", maxSteepness);

            if(GUILayout.Button("Generate trees")){
                TerrainFoliage.maxSteepness = maxSteepness;
                if (GameObject.Find("Water"))
                {
                    TerrainFoliage.waterLevel = GameObject.Find("Water").transform.position.y + waterCoast;
                }
                else
                {
                    TerrainFoliage.waterLevel = 0.0f;
                }
                TerrainFoliage.GenerateFoliage();
            }

            if (GUILayout.Button("Remove trees"))
            {
                TerrainFoliage.ClearTrees();
            }

            grassDensity = EditorGUILayout.IntField("Grass density", grassDensity);

            if (GUILayout.Button("Generate grass"))
            {
                TerrainFoliage.GenerateGrass();
            }

            if (GUILayout.Button("Remove grass"))
            {
                TerrainFoliage.ClearGrass();
            }
        }

        EditorGUILayout.EndToggleGroup();
        
    }
}
