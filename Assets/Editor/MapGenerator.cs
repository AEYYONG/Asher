using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGenerator : EditorWindow
{
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
        _gridPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Grid.prefab");
        
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
    }

    private void OnDisable()
    {
        //커스텀 에디터 창이 끌 때
        //타일 매니저에 해당 타일의 정보 저장
        _tileManager.width = _curWidth;
        _tileManager.height = _curHeight;
        EditorUtility.SetDirty(_tileManager);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    private void OnGUI()
    {
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
                    entry.tile.transform.position = new Vector3(x, 0, z);
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
                    entry.tile.transform.position = new Vector3(x, 0, z);
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
}
