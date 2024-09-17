using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileSelectWindow : EditorWindow
{
    private GameObject selectedPrefab;
    private MapGenerator parentEditor;
    private List<TileTypeStruct> _tileTypeList;
    
    public static void ShowWindow(MapGenerator parent)
    {
        // Create the window and set its title
        TileSelectWindow window = GetWindow<TileSelectWindow>("Select Prefab");
        window.parentEditor = parent;
    }

    private void OnGUI()
    {
        GUILayout.Label("Select a Prefab", EditorStyles.boldLabel);
        selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", selectedPrefab, typeof(GameObject), false);
        
        if (GUILayout.Button("Add"))
        {
            parentEditor.AddTileType(selectedPrefab);
            Close();
        }
    }
}
