using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static MapGenerator;

/// <summary>
/// Custom Editor for MapGenerator Script
/// </summary>
[CustomEditor (typeof (MapGenerator))]
public class MapGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        // Every time a value is changed and autoUpdate is selected, update the map
        if( DrawDefaultInspector())
        {
            if(mapGen.autoUpdate)
            {
                MapData mapData = mapGen.GenerateMapData();
                mapGen.DrawMapInEditor(mapData);
            }
        }

        // Add a Generate button that generates a new map with the current values
        if(GUILayout.Button("Generate"))
        {
            MapData mapData = mapGen.GenerateMapData();
            mapGen.DrawMapInEditor(mapData);
        }

        // Add a Hide button that hides the generated maps
        if (GUILayout.Button("Hide"))
        {
            mapGen.HideMapInEditor();
        }

    }

}
