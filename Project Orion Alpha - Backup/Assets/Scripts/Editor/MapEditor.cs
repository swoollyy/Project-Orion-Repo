using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator meshGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if(meshGen.autoGenerate)
            {
                meshGen.DrawMapInEditor();
            }
        }

        if(GUILayout.Button("Generate"))
        {
            meshGen.DrawMapInEditor();
        }
    }
}
