using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class World : EditorWindow {

    private WorldGen worldGen;

    private Vector2 scrollPos = Vector2.zero;

    [MenuItem("Window/World Builder")]
    static void Init()
    {
        GetWindow<World>();
    }

    void OnEnable()
    {
        hideFlags = HideFlags.HideAndDontSave;
        if (worldGen == null)
        {
            worldGen = ScriptableObject.CreateInstance<WorldGen>();
        }
    }

    void OnGUI()
    {
        try
        {
            GameObject go = GameObject.Find("Terrain");
            if (go.GetComponent<Terrain>() == null)
            {
                EditorGUILayout.HelpBox("Your gameobject 'Terrain' does not have component 'Terrain'", MessageType.Warning);
            }
            else
            {
                GUILayout.Label("World Builder", EditorStyles.boldLabel);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                worldGen.OnGUI();
                EditorGUILayout.EndScrollView();
            }
        }
        catch
        {
            EditorGUILayout.HelpBox("Insert 'Terrain' on the scene", MessageType.Warning);
        }
        
    }

}
