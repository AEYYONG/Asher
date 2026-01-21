using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TilePaintFeatures
{
    public TilePaintFeatures(MapContext ctx)
    {
        //모드 디폴트 값으로 설정
        ctx.curDrawMode = DrawMode.DEFAULT;
        ctx.curTypeMode = TypeMode.DEFAULT;
        
        //타일 타입 텍스쳐 불러오기
        ctx.notAvailTile = AssetDatabase.LoadAssetAtPath<GameObject>(ctx.prefabPath+"NotAvail.prefab");
        ctx.eventTile = AssetDatabase.LoadAssetAtPath<GameObject>(ctx.prefabPath+"Event.prefab");

        CheckTexParent(ctx);
    }

    void CheckTexParent(MapContext ctx)
    {
        // 씬에 Text Parent 오브젝트가 있는지 체크
        if (GameObject.Find("Tex Parent") is GameObject _texParent)
        {
            ctx.texParent = _texParent;
        }
        else
        {
            // text parent로 사용할 빈 오브젝트 생성하고, 이름을 text Parent로 명명하기
            ctx.texParent = new GameObject();
            ctx.texParent.name = "Tex Parent";
        }
    }

    public void DrawTilePaintFeatures(MapContext ctx)
    {
        // 공통 스타일 입히기
        GUIStyles.Ensure();
        
        EditorGUILayout.BeginVertical(GUIStyles.SectionBox);
        GUILayout.Label("Edit Mode", GUIStyles.HeaderLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Draw: {ctx.curDrawMode} / Type: {ctx.curTypeMode}", EditorStyles.miniBoldLabel);

        if (GUILayout.Button("None", GUIStyles.MiniButton, GUILayout.Width(60)))
        {
            ctx.curDrawMode = DrawMode.DEFAULT;
            ctx.curTypeMode = TypeMode.DEFAULT;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(4);

        EditorGUILayout.BeginHorizontal();

        // 선택 타일
        ctx.selectedTile = (GameObject)EditorGUILayout.ObjectField(ctx.selectedTile, typeof(GameObject), false);

        // Brush / Eraser
        if (GUILayout.Toggle(ctx.curDrawMode == DrawMode.BRUSH, "Brush", "Button", GUILayout.Width(70)))
        {
            ctx.curDrawMode = DrawMode.BRUSH;
            ctx.curTypeMode = TypeMode.DEFAULT;
        }
        if (GUILayout.Toggle(ctx.curDrawMode == DrawMode.ERASER, "Eraser", "Button", GUILayout.Width(70)))
        {
            ctx.curDrawMode = DrawMode.ERASER;
            ctx.curTypeMode = TypeMode.DEFAULT;
        }
        
        EditorGUILayout.EndHorizontal();
        
        if (ctx.curDrawMode == DrawMode.BRUSH && ctx.selectedTile == null)
            EditorGUILayout.HelpBox("Brush 모드입니다. 먼저 타일 프리팹을 선택하세요.", MessageType.Warning);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        // 타일 텍스처 보이기
        Texture2D notAvailTex = AssetPreview.GetAssetPreview(ctx.notAvailTile);
        GUILayout.Label(notAvailTex, GUILayout.Width(70), GUILayout.Height(70));
        // 타일 타입 지정
        if (GUILayout.Toggle(ctx.curTypeMode == TypeMode.NOTAVAIL, "NotAvail", "Button", GUILayout.Width(70)))
        {
            ctx.curDrawMode = DrawMode.SELECT;
            ctx.curTypeMode = TypeMode.NOTAVAIL;
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical();
        Texture2D eventTileTex = AssetPreview.GetAssetPreview(ctx.eventTile);
        GUILayout.Label(eventTileTex, GUILayout.Width(70), GUILayout.Height(70));
        if (GUILayout.Toggle(ctx.curTypeMode == TypeMode.EVENT, "Event", "Button", GUILayout.Width(70)))
        {
            ctx.curDrawMode = DrawMode.SELECT;
            ctx.curTypeMode = TypeMode.EVENT;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        Texture2D emptyTex = AssetPreview.GetAssetPreview(GUIStyles.EmptyTex);
        GUILayout.Label(emptyTex, GUILayout.Width(70), GUILayout.Height(70));
        if (GUILayout.Toggle(ctx.curDrawMode == DrawMode.DISSELECT, "Clear", "Button", GUILayout.Width(70)))
        {
            ctx.curDrawMode = DrawMode.DISSELECT;
            ctx.curTypeMode = TypeMode.AVAIL;
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(10);
    }
    
    //타일 브러쉬,지우개 함수
    public void DrawTile(Vector2 mousePos, MapContext ctx)
    {
        //레이 생성
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
        RaycastHit hit;

        //레이캐스트 수행
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Hit object: " + hit.collider.name);
            // 오브젝트를 선택 상태로 표시
            Selection.activeGameObject = hit.collider.gameObject;
            
            if (hit.collider.CompareTag("MapGrid") && ctx.curDrawMode == DrawMode.BRUSH)
            {
                //선택한 오브젝트가 grid 이면서 brush 모드이면 해당 자리에 타일 생성
                int x = (int)hit.transform.position.x;
                int z = (int)hit.transform.position.z;
                Vector2Int pos = new Vector2Int(x, z);
                TileEntry entry = new TileEntry();
                entry.position = pos;
                entry.tile = PrefabUtility.InstantiatePrefab(ctx.selectedTile, ctx.tileParent.transform) as GameObject;
                entry.tile.name = $"Tile({x},{z})";
                entry.tile.GetComponent<Tile>().InitTile(x,z);
                entry.tile.transform.position = new Vector3(x, 0, z);
                EditorUtility.SetDirty(entry.tile);
                ctx.tileManager.tileEntries.Add(entry);
            }
            else if (hit.collider.name.Substring(0,4) == "Tile" &&  ctx.curDrawMode == DrawMode.ERASER)
            {
                //선택한 오브젝트가 tile 이면서 eraser 모드이면 해당 자리의 타일을 삭제
                int x = (int)hit.transform.position.x;
                int z = (int)hit.transform.position.z;
                Vector2Int pos = new Vector2Int(x, z);
                TileEntry remove = new TileEntry();
                foreach (var entry in ctx.tileManager.tileEntries)
                {
                    if (entry.position == pos)
                    {
                        Object.DestroyImmediate(entry.tile);
                        remove = entry;
                    }
                }
                ctx.tileManager.tileEntries.Remove(remove);
            }
        }
        else
        {
            Debug.Log("No object hit.");
        }
    }
    
    public void DrawSelectedPreviewSection(MapContext ctx, EditorWindow window)
    {
        if (ctx.selectedTile == null) return;

        EditorGUILayout.BeginVertical(GUIStyles.SectionBox);
        GUILayout.Label("Selected Tile Preview", GUIStyles.HeaderLabel);

        Texture2D previewTexture = AssetPreview.GetAssetPreview(ctx.selectedTile);
        
        if (previewTexture != null)
        {
            float w = window.position.width - 40;
            float h = Mathf.Min(180, w);
            GUILayout.Label(previewTexture, GUILayout.Width(w), GUILayout.Height(h));
        }
        else
        {
            EditorGUILayout.HelpBox("프리뷰를 생성 중.", MessageType.Info);
            window.Repaint();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(10);
    }

    public void DestroyTexParent(MapContext ctx)
    {
        Object.DestroyImmediate(ctx.texParent);
    }
    
    //타일 영역 지정
    void SetTileType(Vector2 mousePos, MapContext ctx)
    {
        //레이 생성
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
        RaycastHit hit;

        //레이캐스트 수행
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Hit object: " + hit.collider.name);
            // 오브젝트를 선택 상태로 표시
            Selection.activeGameObject = hit.collider.gameObject;
            //레이캐스트에 감지된 타일 오브젝트
            GameObject tile = hit.collider.gameObject;
            //사용할 텍스쳐 오브젝트
            GameObject texType;
            //선택된 텍스쳐 오브젝트
            GameObject target = null;
            //타일 컴포넌트 할당
            Tile t = tile.GetComponent<Tile>();
            //삭제할 TexEntry 할당
            TexEntry remove = new TexEntry();
            
            //타일을 선택한다면
            if (hit.collider.name.Substring(0,4) == "Tile" && ctx.curDrawMode == DrawMode.SELECT )
            {
                if (t.tileType != TileType.RandomAvail)
                {
                    return;
                }
                
                if (ctx.curTypeMode == TypeMode.NOTAVAIL)
                {
                    target = ctx.notAvailTile;
                    t.tileType = TileType.RandomNotAvail;
                }
                else if(ctx.curTypeMode == TypeMode.EVENT)
                {
                    target = ctx.eventTile;
                    t.tileType = TileType.Event;
                }
                
                texType = PrefabUtility.InstantiatePrefab(target,ctx.texParent.transform) as GameObject;
                texType.transform.position = new Vector3(tile.transform.position.x,0.1f,tile.transform.position.z);
                
                //tex list에 추가하기
                TexEntry entry = new TexEntry();
                entry.pos = t.ReturnPos();
                entry.tex = texType;
                ctx.texList.Add(entry);
            }
            else if (t.tileType != TileType.RandomAvail && ctx.curDrawMode == DrawMode.DISSELECT)
            {
                foreach (var entry in ctx.texList)
                {
                    if (entry.pos == t.ReturnPos())
                    {
                        t.tileType = TileType.RandomAvail;
                        Object.DestroyImmediate(entry.tex);
                        remove = entry;
                    }
                }
                ctx.texList.Remove(remove);
            }
            //Tile의 Tile 컴포넌트에 tile type 값 수정 반영하기
            EditorUtility.SetDirty(t);
        }
        else
        {
            Debug.Log("No object hit.");
        }
    }
}
