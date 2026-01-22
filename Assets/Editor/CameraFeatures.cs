using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraFeatures
{
    public CameraFeatures()
    {
        
    }

    public void DrawCameraSection(MapContext ctx)
    {
        //카메라 지정
        EditorGUILayout.BeginVertical(GUIStyles.SectionBox);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Camera Setting",GUIStyles.HeaderLabel);
        if (GUILayout.Button("Setting", GUIStyles.MiniButton, GUILayout.Width(70)))
        {
            SetCameraPos(ctx);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(10);
    }
    //카메라 위치 설정
    void SetCameraPos(MapContext ctx)
    {
        Transform camPos = GameObject.Find("Full Camera").GetComponent<Transform>();

        float xPos = (ctx.curWidth - 1) / 2f;

        Vector3 afterPos = new Vector3(xPos, camPos.position.y, camPos.position.z);
        camPos.position = afterPos;
    }
}
