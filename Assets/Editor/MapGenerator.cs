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
    
    
    private Vector2 scrollPos;
    
    
    
    //타일 매니저 스크립트
    private TileManager _tileManager;
    
    //타일 그리기
    private enum DrawMode
    {
        DEFAULT,
        BRUSH,
        ERASER,
        SELECT,
        DISSELECT
    };
    private DrawMode _curDrawMode;
    private GameObject _selectedTile;
    
    //타일 영역 지정
    private enum TypeMode
    {
        DEFAULT,
        AVAIL,
        NOTAVAIL,
        EVENT
    }
    private TypeMode _curTypeMode;
    private GameObject _notAvail;
    private GameObject _event;
    private List<TexEntry> _texList = new List<TexEntry>();
    private GameObject _texParent;
    
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
        

        // 씬 뷰에서 이벤트를 수신하기 위해 duringSceneGui 이벤트에 핸들러 추가
        SceneView.duringSceneGui += OnSceneGUI;

        //타일 그리기 모드 디폴트 값으로 설정
        _curDrawMode = DrawMode.DEFAULT;
        
        //타일 영역 모드 디폴트 값으로 설정
        _curTypeMode = TypeMode.DEFAULT;
        _curDrawMode = DrawMode.DEFAULT;
        
        //타일 타입 텍스쳐 불러오기
        _notAvail = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/04.Prefabs/CustomEditor/NotAvail.prefab");
        _event = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/04.Prefabs/CustomEditor/Event.prefab");
        _texParent = new GameObject();
        _texParent.name = "Tex Parent";
    }

    private void OnDisable()
    {
        //커스텀 에디터 창이 끌 때
        //타일 매니저에 해당 타일의 정보 저장
        _tileSection.SaveTileData(_ctx);
        
        // 이벤트 핸들러 제거
        SceneView.duringSceneGui -= OnSceneGUI;
        
        DestroyImmediate(_texParent);
        
        EditorUtility.SetDirty(_tileManager);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        
    }
    
    void OnSceneGUI(SceneView sceneView)
    {
        // 마우스 클릭 감지
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0) // 마우스 왼쪽 버튼 클릭
        {
            if (_curDrawMode != DrawMode.DEFAULT)
            {
                DrawTile(e.mousePosition);
            }

            if (_curTypeMode != TypeMode.DEFAULT)
            {
                SetTileType(e.mousePosition);
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
        
        
        
        //타일 그리기 부분
        GUILayout.BeginHorizontal();
        _selectedTile = (GameObject)EditorGUILayout.ObjectField("selected tile", _selectedTile, typeof(GameObject),false);
        if (GUILayout.Button("Brush"))
        {
            //모드 변경
            _curDrawMode = DrawMode.BRUSH;
            Debug.Log("MODE : BRUSH");
        }
        if (GUILayout.Button("Eraser"))
        {
            //모드 변경
            _curDrawMode = DrawMode.ERASER;
            Debug.Log("MODE : ERASER");
        }
        GUILayout.EndHorizontal();
        
        //선택한 타일 미리보기
        if (_selectedTile != null)
        {
            Texture2D previewTexture = AssetPreview.GetAssetPreview(_selectedTile);
            GUILayout.Box(previewTexture,GUILayout.Width(position.width-8));
        }
        
        //타일 영역 지정
        EditorGUILayout.Space(10);
        //타일 생성 부분
        GUILayout.Label("Tile Area",EditorStyles.largeLabel);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        GUILayout.BeginVertical();
        Texture2D notAvailTex = AssetPreview.GetAssetPreview(_notAvail);
        GUILayout.Label(notAvailTex, GUILayout.Width(100), GUILayout.Height(100));
        GUILayout.Label("타일 불가능 영역");
        if (GUILayout.Button("Select"))
        {
            _curDrawMode = DrawMode.SELECT;
            _curTypeMode = TypeMode.NOTAVAIL;
            Debug.Log("MODE : SELECT, NOTAVAIL");
        }
        if (GUILayout.Button("DisSelect"))
        {
            _curDrawMode = DrawMode.DISSELECT;
            _curTypeMode = TypeMode.AVAIL;
            Debug.Log("MODE : DISSELECT, AVAIL");
        }
        GUILayout.EndVertical();
        
        GUILayout.FlexibleSpace();
        
        GUILayout.BeginVertical();
        Texture2D eventTex = AssetPreview.GetAssetPreview(_event);
        GUILayout.Label(eventTex, GUILayout.Width(100), GUILayout.Height(100));
        GUILayout.Label("이벤트 타일 영역");
        if (GUILayout.Button("Select"))
        {
            _curDrawMode = DrawMode.SELECT;
            _curTypeMode = TypeMode.EVENT;
            Debug.Log("MODE : SELECT, EVENT");
        }
        if (GUILayout.Button("DisSelect"))
        {
            _curDrawMode = DrawMode.DISSELECT;
            _curTypeMode = TypeMode.AVAIL;
            Debug.Log("MODE : DISSELECT, AVAIL");
        }
        GUILayout.EndVertical();
        
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        
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
    

    
    
    
    
    //타일 브러쉬,지우개 함수
    void DrawTile(Vector2 mousePos)
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
            
            if (hit.collider.CompareTag("MapGrid") && _curDrawMode == DrawMode.BRUSH)
            {
                //선택한 오브젝트가 grid 이면서 brush 모드이면 해당 자리에 타일 생성
                int x = (int)hit.transform.position.x;
                int z = (int)hit.transform.position.z;
                Vector2Int pos = new Vector2Int(x, z);
                TileEntry entry = new TileEntry();
                entry.position = pos;
                entry.tile = PrefabUtility.InstantiatePrefab(_selectedTile,_ctx.tileParent.transform) as GameObject;
                entry.tile.name = $"Tile({x},{z})";
                entry.tile.GetComponent<Tile>().InitTile(x,z);
                entry.tile.transform.position = new Vector3(x, 0, z);
                EditorUtility.SetDirty(entry.tile);
                _tileManager.tileEntries.Add(entry);
            }
            else if (hit.collider.name.Substring(0,4) == "Tile" &&  _curDrawMode == DrawMode.ERASER)
            {
                //선택한 오브젝트가 tile 이면서 eraser 모드이면 해당 자리의 타일을 삭제
                int x = (int)hit.transform.position.x;
                int z = (int)hit.transform.position.z;
                Vector2Int pos = new Vector2Int(x, z);
                TileEntry remove = new TileEntry();
                foreach (var entry in _tileManager.tileEntries)
                {
                    if (entry.position == pos)
                    {
                        DestroyImmediate(entry.tile);
                        remove = entry;
                    }
                }
                _tileManager.tileEntries.Remove(remove);
            }
        }
        else
        {
            Debug.Log("No object hit.");
        }
    }
    
    //타일 영역 지정
    void SetTileType(Vector2 mousePos)
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
            if (hit.collider.name.Substring(0,4) == "Tile" && _curDrawMode == DrawMode.SELECT )
            {
                if (t.tileType != TileType.RandomAvail)
                {
                    return;
                }
                
                if (_curTypeMode == TypeMode.NOTAVAIL)
                {
                    target = _notAvail;
                    t.tileType = TileType.RandomNotAvail;
                }
                else if(_curTypeMode == TypeMode.EVENT)
                {
                    target = _event;
                    t.tileType = TileType.Event;
                }
                
                texType = PrefabUtility.InstantiatePrefab(target,_texParent.transform) as GameObject;
                texType.transform.position = new Vector3(tile.transform.position.x,0.1f,tile.transform.position.z);
                
                //tex list에 추가하기
                TexEntry entry = new TexEntry();
                entry.pos = t.ReturnPos();
                entry.tex = texType;
                _texList.Add(entry);
            }
            else if (t.tileType != TileType.RandomAvail && _curDrawMode == DrawMode.DISSELECT)
            {
                foreach (var entry in _texList)
                {
                    if (entry.pos == t.ReturnPos())
                    {
                        t.tileType = TileType.RandomAvail;
                        DestroyImmediate(entry.tex);
                        remove = entry;
                    }
                }
                _texList.Remove(remove);
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
