using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct TexEntry
{
    public Vector2Int pos;
    public GameObject tex;
}
public class MapGenerator : EditorWindow
{
    private MapContext _ctx;
    private GridFeatures _gridSection;
    private TileFeatures _tileSection;
    private TilePaintFeatures _tilePaintSection;
    
    
    private Vector2 scrollPos;
    
    
    
    //타일 매니저 스크립트
    private TileManager _tileManager;
    
    
    
    //타일 영역 지정
    
    
    
    //타일 종류
    private int _totalTileCnt;
    private bool _buffItemStatus;
    private bool _debuffItemStatus;
    private bool _etcItemStatus;
    private List<TileTypeStruct> _curTileTypeList;
    
    
    //윈도우 메뉴에 "Map Generator"
    [MenuItem("Window/Map Generator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MapGenerator));
    }

    private void OnEnable()
    {
        if (_ctx == null)
        {
            _ctx = new MapContext();
        }

        _tileManager = FindObjectOfType<TileManager>();
        _gridSection = new GridFeatures(_ctx);
        _tileSection = new TileFeatures(_ctx);
        _tilePaintSection = new TilePaintFeatures(_ctx);
        

        // 씬 뷰에서 이벤트를 수신하기 위해 duringSceneGui 이벤트에 핸들러 추가
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        //커스텀 에디터 창이 끌 때
        //타일 매니저에 해당 타일의 정보 저장
        _tileSection.SaveTileData(_ctx);
        
        // 이벤트 핸들러 제거
        SceneView.duringSceneGui -= OnSceneGUI;

        _tilePaintSection.DestroyTexParent(_ctx);
        
        EditorUtility.SetDirty(_tileManager);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        
    }
    
    void OnSceneGUI(SceneView sceneView)
    {
        // 마우스 클릭 감지
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0) // 마우스 왼쪽 버튼 클릭
        {
            if (_ctx.curDrawMode != DrawMode.DEFAULT)
            {
                _tilePaintSection.DrawTile(e.mousePosition, _ctx);
            }

            if (_ctx.curTypeMode != TypeMode.DEFAULT)
            {
                _tilePaintSection.SetTileType(e.mousePosition, _ctx);
            }
            e.Use(); // 이벤트 처리 완료 표시
        }
    }

    private void OnGUI()
    {
        //스크롤 포지션 할당
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        _gridSection.DrawGridSection(_ctx);
        _tileSection.DrawTileSection(_ctx);
        _tilePaintSection.DrawTilePaintFeatures(_ctx, this);
        
        //타일 종류
        EditorGUILayout.Space(10);
        GUILayout.Label("Tile Type",EditorStyles.largeLabel);
        GUILayout.Label("Current Tiles : " + CalculateTileCnt());
        
        //기타 아이템 추가
        _etcItemStatus = EditorGUILayout.Foldout(_etcItemStatus, "Etc Items");
        if (_etcItemStatus)
        {
            if (GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
            {
                _curTileTypeList = _tileManager.etcItemTypes;
                TileSelectWindow.ShowWindow(this);
            }
            //디버프 아이템 리스트 순회
            for(int i=0; i<_tileManager.etcItemTypes.Count; i++)
            {
                TileTypeStruct item = _tileManager.etcItemTypes[i];
                GUILayout.BeginHorizontal();
                //타일 프리팹이 있으면 미리보기 제공
                if (item.tilePrefab != null)
                {
                    Texture2D itemTex = AssetPreview.GetAssetPreview(item.tilePrefab);
                    GUILayout.Label(itemTex, GUILayout.Width(100), GUILayout.Height(100));   
                }
                GUILayout.BeginVertical();
                GUILayout.Label(item.tilePrefab.name);
                item.count = EditorGUILayout.IntField("count", item.count);
                GUILayout.Space(40);
                //삭제 함수
                if (GUILayout.Button("Delete"))
                {
                    _tileManager.etcItemTypes.Remove(item);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }
        
        _buffItemStatus = EditorGUILayout.Foldout(_buffItemStatus, "Buff Items");
        //버프 아이템 추가
        if (_buffItemStatus)
        {
            //버프 아이템 프리팹 추가 버튼
            if (GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
            {
                //현재 타입 리스트를 버프 아이템으로 설정
                _curTileTypeList = _tileManager.buffItemTypes;
                //타일 선택 창 열기
                TileSelectWindow.ShowWindow(this);
            }

            //버프 아이템 리스트 순회
            for(int i=0; i<_tileManager.buffItemTypes.Count; i++)
            {
                TileTypeStruct item = _tileManager.buffItemTypes[i];
                GUILayout.BeginHorizontal();
                //타일 프리팹이 있으면 미리보기 제공
                if (item.tilePrefab != null)
                {
                    Texture2D itemTex = AssetPreview.GetAssetPreview(item.tilePrefab);
                    GUILayout.Label(itemTex, GUILayout.Width(100), GUILayout.Height(100));   
                }
                GUILayout.BeginVertical();
                GUILayout.Label(item.tilePrefab.name);
                item.count = EditorGUILayout.IntField("count", item.count);
                GUILayout.Space(40);
                //삭제 함수
                if (GUILayout.Button("Delete"))
                {
                    _tileManager.buffItemTypes.Remove(item);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }
        
        //디버프 아이템 추가
        _debuffItemStatus = EditorGUILayout.Foldout(_debuffItemStatus, "DeBuff Items");
        if (_debuffItemStatus)
        {
            if (GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
            {
                _curTileTypeList = _tileManager.debuffItemTypes;
                TileSelectWindow.ShowWindow(this);
            }
            //디버프 아이템 리스트 순회
            for(int i=0; i<_tileManager.debuffItemTypes.Count; i++)
            {
                TileTypeStruct item = _tileManager.debuffItemTypes[i];
                GUILayout.BeginHorizontal();
                //타일 프리팹이 있으면 미리보기 제공
                if (item.tilePrefab != null)
                {
                    Texture2D itemTex = AssetPreview.GetAssetPreview(item.tilePrefab);
                    GUILayout.Label(itemTex, GUILayout.Width(100), GUILayout.Height(100));   
                }
                GUILayout.BeginVertical();
                GUILayout.Label(item.tilePrefab.name);
                item.count = EditorGUILayout.IntField("count", item.count);
                GUILayout.Space(40);
                //삭제 함수
                if (GUILayout.Button("Delete"))
                {
                    _tileManager.debuffItemTypes.Remove(item);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.Space(10);
        
        //카메라 지정
        GUILayout.Label("Camera Setting",EditorStyles.largeLabel);
        if (GUILayout.Button("Setting"))
        {
            SetCameraPos();
        }
        EditorGUILayout.EndScrollView();
    }

    //타일 타입 추가
    public void AddTileType(GameObject prefab)
    {
        TileTypeStruct type = new TileTypeStruct();
        type.tilePrefab = prefab;
        type.count = 0;
        _curTileTypeList.Add(type);
    }

    //카메라 위치 설정
    void SetCameraPos()
    {
        
    }
    
    //총 타일 수 계산
    int CalculateTileCnt()
    {
        _totalTileCnt = 0;
        foreach (var entry in _tileManager.tileEntries)
        {  
            TileType type = entry.tile.GetComponent<Tile>().tileType;

            //타일 배치 가능
            if (type == TileType.RandomAvail)
            {
                _totalTileCnt += 1;
            }
        }

        return _totalTileCnt;
    }
}
