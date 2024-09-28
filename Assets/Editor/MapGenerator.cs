using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Unity.VisualScripting;
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
    private Vector2 scrollPos;
    
    //Grid 생성
    private int _gridWidth; //grid 가로 개수
    private int _gridHeight; //grid 세로 개수 
    private GameObject _gridPrefab; //grid prefab
    private GameObject _gridParent; //생성된 grid들이 들어갈 부모 오브젝트
    private List<GameObject> _gridList = new List<GameObject>(); //grid 오브젝트들을 저장할 리스트
    
    //타일 생성
    private int _prevWidth; //이전 타일 너비
    private int _prevHeight; //이전 타일 높이
    private int _curWidth; //타일 너비
    private int _curHeight; //타일 높이
    private GameObject _tilePrefab; //타일 프리팹
    private GameObject _tileParent; //타일들이 생성될 부모 오브젝트
    
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
        //해당 씬 내의 tile manager를 통해 값 초기화 하기
        _tileManager = FindObjectOfType<TileManager>();
        _prevHeight = _tileManager.height;
        _prevWidth = _tileManager.width;
        _curHeight = _prevHeight;
        _curWidth = _prevWidth;
        
        //grid prefab 초기화
        _gridPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/CustomEditor/Grid.prefab");
        
        //초기화 확인
        if (_tilePrefab == null)
        {
            Debug.Log("tile prefab does not initiate.");
        }
        else if (_tileParent == null)
        {
            Debug.Log("tile parent does not initiate");
        }
        else if (_tileManager == null)
        {
            Debug.Log("tile manager does not initiate");
        }

        if (_curWidth == 0 && _curHeight == 0)
        {
            _tileManager.tileEntries.Clear();
        }

        // 씬 뷰에서 이벤트를 수신하기 위해 duringSceneGui 이벤트에 핸들러 추가
        SceneView.duringSceneGui += OnSceneGUI;

        //타일 그리기 모드 디폴트 값으로 설정
        _curDrawMode = DrawMode.DEFAULT;
        
        //타일 영역 모드 디폴트 값으로 설정
        _curTypeMode = TypeMode.DEFAULT;
        _curDrawMode = DrawMode.DEFAULT;
        
        //타일 타입 텍스쳐 불러오기
        _notAvail = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/CustomEditor/NotAvail.prefab");
        _event = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/CustomEditor/Event.prefab");
        _texParent = new GameObject();
        _texParent.name = "Tex Parent";
    }

    private void OnDisable()
    {
        //커스텀 에디터 창이 끌 때
        //타일 매니저에 해당 타일의 정보 저장
        _tileManager.width = _curWidth;
        _tileManager.height = _curHeight;
        
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
        //grid 생성 부분
        GUILayout.Label("Generate Grid",EditorStyles.largeLabel);
        _gridWidth = EditorGUILayout.IntField("grid width", _gridWidth);
        _gridHeight = EditorGUILayout.IntField("grid height", _gridHeight);
        //generate 버튼 클릭 시, grid 생성 함수 호출
        if (GUILayout.Button("Generate Grid"))
        {
            GenerateGrid(_gridWidth,_gridHeight);
        }
        //destroy 버튼 클릭 시, grid 삭제 함수 호출
        if (GUILayout.Button("Destroy Grid"))
        {
            DestroyGrid();
        }
        EditorGUILayout.Space(10);
        //타일 생성 부분
        GUILayout.Label("Generate Tile",EditorStyles.largeLabel);
        _curWidth = EditorGUILayout.IntSlider("width",_curWidth,0,20);
        _curHeight = EditorGUILayout.IntSlider("height",_curHeight,0,20);
        _tilePrefab = (GameObject)EditorGUILayout.ObjectField("tile", _tilePrefab, typeof(GameObject),false);
        _tileParent = (GameObject)EditorGUILayout.ObjectField("tile parent", _tileParent, typeof(GameObject), true);
        EditorGUILayout.Space();
        
        //타일의 이전 너비/높이와 현재 너비/높이가 달라졌을 경우 타일 생성/삭제 함수 호출
        if (_prevWidth != _curWidth)
        {
            //가로 값이 달라졌다면
            GenerateTile();
            _prevWidth = _curWidth;
        }
        if (_prevHeight != _curHeight)
        {
            //세로 값이 달라졌다면
            GenerateTile();
            _prevHeight = _curHeight;
        }
        
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
    

    //Grid Map 생성하기
    void GenerateGrid(int w, int h)
    {
        //grid parent로 사용할 빈 오브젝트 생성하고, 이름을 Grid Parent로 명명하기
        _gridParent = new GameObject();
        _gridParent.name = "Grid Parent";
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                //grid를 gird parent의 자식으로 생성한 후, grid 리스트에 넣기
                GameObject grid = PrefabUtility.InstantiatePrefab(_gridPrefab, _gridParent.transform) as GameObject;
                grid.transform.position = new Vector3(j, -0.1f, i);
                _gridList.Add(grid);
            }
        }
    }
    //Grid Map 삭제하기
    void DestroyGrid()
    {
        //씬에서 Grid Parent 이름의 오브젝트를 찾기
        GameObject gridParent = GameObject.Find("Grid Parent");
        if (gridParent == null)
        {
            Debug.Log("Grid Parent가 없습니다");
        }
        else
        {
            //Grid Parent를 삭제하며 리스트 초기화
            DestroyImmediate(gridParent);
            _gridList.Clear();
        }
    }
    
    //타일 증가 및 감소 관리 함수
    void GenerateTile()
    {
        //현재 타일 너비가 직전 타일 너비보다 크다면 -> 타일을 증가하겠다는 의도
        if (_curWidth > _prevWidth)
        {
            //타일이 가로로 증가
            //현재 높이만큼 채우기
            for (int i = 0; i < _curHeight; i++)
            {
                //이미 채워져있던 너비 이후 만큼 채우기
                for (int j = 0; j < _curWidth - _prevWidth; j++)
                {
                    //현재 너비 - 이전 너비를 하여 추가되어야 하는 가로 타일 개수(j)를 계산
                    int x = _prevWidth + j;
                    int z = i;
                    Vector2Int pos = new Vector2Int(x, z);
                    //TileEntry 생성 후, 타일 매니저 내의 타일 리스트에 추가하기
                    TileEntry entry = new TileEntry();
                    entry.position = pos;
                    entry.tile = PrefabUtility.InstantiatePrefab(_tilePrefab,_tileParent.transform) as GameObject;
                    entry.tile.name = $"Tile({x},{z})";
                    entry.tile.GetComponent<Tile>().InitTile(x,z);
                    entry.tile.transform.position = new Vector3(x, 0, z);
                    EditorUtility.SetDirty(entry.tile);
                    _tileManager.tileEntries.Add(entry);
                }
            }
        }
        else if(_curWidth < _prevWidth) //현재 너비가 이전 너비보다 작다면 -> 타일을 가로로 감소하려는 의도
        {
            //삭제할 타일 엔트리 리스트를 생성, foreach 내에서 리스트 삭제 시 예외처리 발생하기 때문임.
            //컬렉션을 순회하면서 동시에 수정하려고 하면 런타임 예외가 발생함.
            List<TileEntry> removeEntries = new List<TileEntry>();
            //타일이 가로로 감소
            for (int i = 0; i <_curHeight; i++)
            {
                for (int j = 0; j < _prevWidth-_curWidth; j++)
                {
                    int x = _prevWidth -1 - j;
                    int z = i;
                    Vector2Int pos = new Vector2Int(x, z);
                    foreach (var entry in _tileManager.tileEntries)
                    {
                        if (entry.position == pos)
                        {
                            DestroyImmediate(entry.tile);
                            removeEntries.Add(entry);
                        }
                    }
                }
            }
            //리스트 내에서 타일 엔트리 삭제
            foreach (var entry in removeEntries)
            {
                _tileManager.tileEntries.Remove(entry);
            }
            
        }
        
        //현재 높이가 이전 높이보다 크다면 -> 높이를 증가하려는 의도
        if (_curHeight > _prevHeight)
        {
            //타일이 세로로 증가
            for (int i = 0; i < _curHeight - _prevHeight; i++)
            {
                for (int j = 0; j < _curWidth; j++)
                {
                    int x = j;
                    int z = _prevHeight + i;
                    Vector2Int pos = new Vector2Int(x, z);
                    TileEntry entry = new TileEntry();
                    entry.position = pos;
                    entry.tile = PrefabUtility.InstantiatePrefab(_tilePrefab,_tileParent.transform) as GameObject;
                    entry.tile.name = $"Tile({x},{z})";
                    entry.tile.GetComponent<Tile>().InitTile(x,z);
                    entry.tile.transform.position = new Vector3(x, 0, z);
                    EditorUtility.SetDirty(entry.tile);
                    _tileManager.tileEntries.Add(entry);
                }
            }
        }
        else if(_curHeight < _prevHeight) //현재 높이가 이전 높이보다 작다면 -> 높이를 감소하려는 의도
        {
            List<TileEntry> removeEntries = new List<TileEntry>();
            //타일이 세로로 감소
            for (int i = 0; i < _prevHeight - _curHeight; i++)
            {
                for (int j = 0; j < _curWidth; j++)
                {
                    int x = j;
                    int z = _prevHeight -1 - i;
                    Vector2Int pos = new Vector2Int(x, z);
                    foreach (var entry in _tileManager.tileEntries)
                    {
                        if (entry.position == pos)
                        {
                            DestroyImmediate(entry.tile);
                            removeEntries.Add(entry);
                        }
                    }
                }
            }
            foreach (var entry in removeEntries)
            {
                _tileManager.tileEntries.Remove(entry);
            }
        }
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
                entry.tile = PrefabUtility.InstantiatePrefab(_selectedTile,_tileParent.transform) as GameObject;
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
                }
                else if(_curTypeMode == TypeMode.EVENT)
                {
                    target = _event;
                }
                
                t.tileType = (TileType)_curTypeMode;
                tile.GetComponent<Tile>().tileType = (TileType)_curTypeMode;
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
                        t.tileType = (TileType)TypeMode.AVAIL;
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
