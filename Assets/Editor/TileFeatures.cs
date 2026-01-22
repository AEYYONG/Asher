using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileFeatures
{
    public TileFeatures(MapContext ctx)
    {
        //초기화 확인
        if (ctx.tilePrefab == null)
        {
            ctx.tilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ctx.prefabPath + "GeneralTile.prefab");
        }
        
        if (ctx.tileManager == null)
        {
            ctx.tileManager = Object.FindObjectOfType<TileManager>();
        }
        
        CheckTileParent(ctx);
    }
    
    // Tiles 오브젝트가 있는지 없는지 체크하는 함수
    void CheckTileParent(MapContext ctx)
    {
        // 씬에 Tiles 오브젝트가 있는지 체크
        if (GameObject.Find("Tiles") is GameObject _tileParent)
        {
            ctx.tileParent = _tileParent;
            CalculateTileSize(ctx);
        }
        else
        {
            // Tile Parent 사용할 빈 오브젝트 생성하고, 이름을 Tiles 명명하기
            ctx.tileParent = new GameObject();
            ctx.tileParent.name = "Tiles";

            InitTileData(ctx);
        }
    }

    void InitTileData(MapContext ctx)
    {
        ctx.curWidth = 0;
        ctx.curHeight = 0;
        ctx.prevWidth = 0;
        ctx.prevHeight = 0;
        
        ctx.tileManager.tileEntries.Clear();
    }
    
    // 타일 매니저의 타일 width, height 값을 반영하는 함수
    public void SaveTileData(MapContext ctx)
    {
        ctx.tileManager.width = ctx.curWidth;
        ctx.tileManager.height = ctx.curHeight;
    }

    public void DrawTileSection(MapContext ctx)
    {
        // 공통 스타일 적용
        GUIStyles.Ensure();
        
        EditorGUILayout.BeginVertical(GUIStyles.SectionBox);
        GUILayout.Label("Generate Tile", GUIStyles.HeaderLabel);

        ctx.curWidth = EditorGUILayout.IntSlider("Width",ctx.curWidth,0,20);
        ctx.curHeight = EditorGUILayout.IntSlider("Height",ctx.curHeight,0,20);
        
        // 설명 박스
        EditorGUILayout.HelpBox(
            "슬라이더를 조절하여 타일을 직사각형 형태로 생성할 수 있습니다.",
            MessageType.Info
        );
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(10);

        // 슬라이더에 변화가 생기는지 감지하는 함수
        CheckTileEdits(ctx);
    }

    // 슬라이더에 변화(수정사항)이 생길 경우 호출되는 함수
    void CheckTileEdits(MapContext ctx)
    {
        //타일의 이전 너비/높이와 현재 너비/높이가 달라졌을 경우 타일 생성/삭제 함수 호출
        if (ctx.prevWidth != ctx.curWidth)
        {
            //가로 값이 달라졌다면
            GenerateTile(ctx);
            ctx.prevWidth = ctx.curWidth;
        }
        if (ctx.prevHeight != ctx.curHeight)
        {
            //세로 값이 달라졌다면
            GenerateTile(ctx);
            ctx.prevHeight = ctx.curHeight;
        }
    }
    
    // 타일 증가 및 감소 관리 함수
    void GenerateTile(MapContext ctx)
    {
        //현재 타일 너비가 직전 타일 너비보다 크다면 -> 타일을 증가하겠다는 의도
        if (ctx.curWidth > ctx.prevWidth)
        {
            //타일이 가로로 증가
            //현재 높이만큼 채우기
            for (int i = 0; i < ctx.curHeight; i++)
            {
                //이미 채워져있던 너비 이후 만큼 채우기
                for (int j = 0; j < ctx.curWidth - ctx.prevWidth; j++)
                {
                    //현재 너비 - 이전 너비를 하여 추가되어야 하는 가로 타일 개수(j)를 계산
                    int x = ctx.prevWidth + j;
                    int z = i;
                    Vector2Int pos = new Vector2Int(x, z);
                    
                    //TileEntry 생성 후, 타일 매니저 내의 타일 리스트에 추가하기
                    TileEntry entry = new TileEntry();
                    entry.position = pos;
                    entry.tile = PrefabUtility.InstantiatePrefab(ctx.tilePrefab,ctx.tileParent.transform) as GameObject;
                    entry.tile.name = $"Tile({x},{z})";
                    entry.tile.GetComponent<Tile>().InitTile(x,z);
                    entry.tile.transform.position = new Vector3(x, 0, z);
                    ctx.tileManager.tileEntries.Add(entry);
                    
                    // 씬에 수정사항이 생겼음을 알려주기
                    EditorUtility.SetDirty(entry.tile);
                }
            }
        }
        else if(ctx.curWidth < ctx.prevWidth) //현재 너비가 이전 너비보다 작다면 -> 타일을 가로로 감소하려는 의도
        {
            //삭제할 타일 엔트리 리스트를 생성, foreach 내에서 리스트 삭제 시 예외처리 발생하기 때문임.
            //컬렉션을 순회하면서 동시에 수정하려고 하면 런타임 예외가 발생함.
            List<TileEntry> removeEntries = new List<TileEntry>();
            
            //타일이 가로로 감소
            for (int i = 0; i <ctx.curHeight; i++)
            {
                for (int j = 0; j < ctx.prevWidth-ctx.curWidth; j++)
                {
                    int x = ctx.prevWidth -1 - j;
                    int z = i;
                    Vector2Int pos = new Vector2Int(x, z);
                    
                    foreach (var entry in ctx.tileManager.tileEntries)
                    {
                        if (entry.position == pos)
                        {
                            Object.DestroyImmediate(entry.tile);
                            removeEntries.Add(entry);
                        }
                    }
                }
            }
            //리스트 내에서 타일 엔트리 삭제
            foreach (var entry in removeEntries)
            {
                ctx.tileManager.tileEntries.Remove(entry);
            }
            
        }
        
        //현재 높이가 이전 높이보다 크다면 -> 높이를 증가하려는 의도
        if (ctx.curHeight > ctx.prevHeight)
        {
            //타일이 세로로 증가
            for (int i = 0; i < ctx.curHeight - ctx.prevHeight; i++)
            {
                for (int j = 0; j < ctx.curWidth; j++)
                {
                    int x = j;
                    int z = ctx.prevHeight + i;
                    Vector2Int pos = new Vector2Int(x, z);
                    
                    TileEntry entry = new TileEntry();
                    entry.position = pos;
                    entry.tile = PrefabUtility.InstantiatePrefab(ctx.tilePrefab,ctx.tileParent.transform) as GameObject;
                    entry.tile.name = $"Tile({x},{z})";
                    entry.tile.GetComponent<Tile>().InitTile(x,z);
                    entry.tile.transform.position = new Vector3(x, 0, z);
                    ctx.tileManager.tileEntries.Add(entry);
                    
                    // 씬에 수정사항이 생겼다고 알리기
                    EditorUtility.SetDirty(entry.tile);
                }
            }
        }
        else if(ctx.curHeight < ctx.prevHeight) //현재 높이가 이전 높이보다 작다면 -> 높이를 감소하려는 의도
        {
            List<TileEntry> removeEntries = new List<TileEntry>();
            //타일이 세로로 감소
            for (int i = 0; i < ctx.prevHeight - ctx.curHeight; i++)
            {
                for (int j = 0; j < ctx.curWidth; j++)
                {
                    int x = j;
                    int z = ctx.prevHeight -1 - i;
                    Vector2Int pos = new Vector2Int(x, z);
                    
                    foreach (var entry in ctx.tileManager.tileEntries)
                    {
                        if (entry.position == pos)
                        {
                            Object.DestroyImmediate(entry.tile);
                            removeEntries.Add(entry);
                        }
                    }
                }
            }
            foreach (var entry in removeEntries)
            {
                ctx.tileManager.tileEntries.Remove(entry);
            }
        }
    }
    
    // 갯수 계산해서 에디터 창 슬라이더와 동기화하는 함수
    void CalculateTileSize(MapContext ctx)
    {
        // tiles 에 자식 오브젝트 없으면 0,0으로 설정
        if (ctx.tileParent.transform.childCount == 0)
        {
            ctx.curWidth = 0;
            ctx.curHeight = 0;
            return;
        }

        int maxX = -1;
        int maxZ = -1;
        // tiles 자식 오브젝트 순회하면서 가장 큰 좌표 값으로 width, height 알아내기
        foreach (Transform tile in (ctx.tileParent.transform))
        {
            Vector3 pos = tile.position;

            int x = Mathf.RoundToInt(pos.x);
            int z = Mathf.RoundToInt(pos.z);

            if (x > maxX) maxX = x;
            if (z > maxZ) maxZ = z;
        }

        ctx.curWidth = maxX + 1;
        ctx.curHeight = maxZ + 1;
        
        ctx.prevWidth = ctx.curWidth;
        ctx.prevHeight = ctx.curHeight;
    }
}
