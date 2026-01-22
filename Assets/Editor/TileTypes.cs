using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TileTypes
{
    public TileTypes()
    {
        
    }

    public void DrawTileTypesSection(MapContext ctx, MapGenerator window)
    {
        EditorGUILayout.BeginVertical(GUIStyles.SectionBox);
        GUILayout.Label("Tile Type", GUIStyles.HeaderLabel);
        GUILayout.Label($"Total Tiles : {CalculateTileCnt(ctx)}", EditorStyles.miniBoldLabel);
        EditorGUILayout.Space(6);

        DrawTileTypeCategory(ref ctx.etcItemStatus, "Etc Items", ctx.tileManager.etcItemTypes,
            () => { ctx.curTileTypeList = ctx.tileManager.etcItemTypes; TileSelectWindow.ShowWindow(window); });

        EditorGUILayout.Space(4);

        DrawTileTypeCategory(ref ctx.buffItemStatus, "Buff Items", ctx.tileManager.buffItemTypes,
            () => { ctx.curTileTypeList = ctx.tileManager.buffItemTypes; TileSelectWindow.ShowWindow(window); });

        EditorGUILayout.Space(4);

        DrawTileTypeCategory(ref ctx.debuffItemStatus, "DeBuff Items", ctx.tileManager.debuffItemTypes,
            () => { ctx.curTileTypeList = ctx.tileManager.debuffItemTypes; TileSelectWindow.ShowWindow(window); });

        // 설명 박스
        EditorGUILayout.HelpBox(
            "맵에 등장할 타일 종류와 개수를 설정합니다. 랜덤 배치 시 사용되는 데이터입니다." +
            "\n타일 전체 개수와 일치하게 지정해주세요.",
            MessageType.Info
        );
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(10);
    }
    
    private void DrawTileTypeCategory(ref bool foldout, string title, List<TileTypeStruct> list, Action onAdd)
    {
        EditorGUILayout.BeginHorizontal();
        foldout = EditorGUILayout.Foldout(foldout, $"{title} ({list.Count})", true);

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+ Add", GUIStyles.MiniButton, GUILayout.Width(70)))
            onAdd?.Invoke();
        EditorGUILayout.EndHorizontal();

        if (!foldout) return;

        // 헤더
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Preview", GUILayout.Width(70));
        GUILayout.Label("Name", GUILayout.Width(180));
        GUILayout.Label("Count", GUILayout.Width(80));
        GUILayout.FlexibleSpace();
        GUILayout.Label("", GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];

            EditorGUILayout.BeginHorizontal(GUIStyles.RowBox);

            if (item.tilePrefab != null)
            {
                Texture2D tex = AssetPreview.GetAssetPreview(item.tilePrefab);
                GUILayout.Label(tex, GUILayout.Width(64), GUILayout.Height(64));
            }
            else
            {
                GUILayout.Box("", GUILayout.Width(64), GUILayout.Height(64));
            }

            string nm = item.tilePrefab ? item.tilePrefab.name : "(Missing)";
            GUILayout.Label(nm, GUILayout.Width(180));

            item.count = EditorGUILayout.IntField(item.count, GUILayout.Width(80));

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Delete", GUIStyles.MiniButton, GUILayout.Width(60)))
            {
                list.RemoveAt(i);
                i--;
                EditorGUILayout.EndHorizontal();
                continue;
            }

            EditorGUILayout.EndHorizontal();
            
            if (i >= 0 && i < list.Count)
                list[i] = item;
        }
    }
    
    //타일 타입 추가
    public void AddTileType(MapContext ctx, GameObject prefab)
    {
        TileTypeStruct type = new TileTypeStruct();
        type.tilePrefab = prefab;
        type.count = 0;
        ctx.curTileTypeList.Add(type);
    }
    
    //총 타일 수 계산
    int CalculateTileCnt(MapContext ctx)
    {
        ctx.totalTileCnt = 0;
        foreach (var entry in ctx.tileManager.tileEntries)
        {  
            TileType type = entry.tile.GetComponent<Tile>().tileType;

            //타일 배치 가능
            if (type == TileType.RandomAvail)
            {
                ctx.totalTileCnt += 1;
            }
        }

        return ctx.totalTileCnt;
    }
}
