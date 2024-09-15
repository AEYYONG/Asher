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
    //타일 생성
    private int _prevWidth;
    private int _prevHeight;
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
        _tileManager.width = _curWidth;
        _tileManager.height = _curHeight;
        EditorUtility.SetDirty(_tileManager);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate Tile",EditorStyles.largeLabel);
        _curWidth = EditorGUILayout.IntSlider("width",_curWidth,0,20);
        _curHeight = EditorGUILayout.IntSlider("height",_curHeight,0,20);
        _tilePrefab = (GameObject)EditorGUILayout.ObjectField("tile", _tilePrefab, typeof(GameObject),false);
        _tileParent = (GameObject)EditorGUILayout.ObjectField("tile parent", _tileParent, typeof(GameObject), true);
        EditorGUILayout.Space();
        
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

    void GenerateTile()
    {
        if (_curWidth > _prevWidth)
        {
            //타일이 가로로 증가
            for (int i = 0; i < _curHeight; i++)
            {
                for (int j = 0; j < _curWidth - _prevWidth; j++)
                {
                    int x = _prevWidth + j;
                    int z = i;
                    Vector2Int pos = new Vector2Int(x, z);
                    Debug.Log(pos+"추가");
                    TileEntry entry = new TileEntry();
                    entry.position = pos;
                    entry.tile = PrefabUtility.InstantiatePrefab(_tilePrefab,_tileParent.transform) as GameObject;
                    entry.tile.name = $"Tile({x},{z})";
                    entry.tile.transform.position = new Vector3(x, 0, z);
                    _tileManager.tileEntries.Add(entry);
                }
            }
        }
        else if(_curWidth < _prevWidth)
        {
            List<TileEntry> removeEntries = new List<TileEntry>();
            //타일이 가로로 감소
            for (int i = 0; i <_curHeight; i++)
            {
                for (int j = 0; j < _prevWidth-_curWidth; j++)
                {
                    int x = _prevWidth -1 - j;
                    int z = i;
                    Vector2Int pos = new Vector2Int(x, z);
                    Debug.Log(pos+"감소");
                    foreach (var entry in _tileManager.tileEntries)
                    {
                        if (entry.position == pos)
                        {
                            Debug.Log(pos+"제거");
                            DestroyImmediate(entry.tile);
                            removeEntries.Add(entry);
                        }
                    }
                }
            }

            foreach (var entry in removeEntries)
            {
                _tileManager.tileEntries.Remove(entry);
                Debug.Log("리무브 엔트리?");
            }
        }
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
                    Debug.Log(pos+"추가");
                    TileEntry entry = new TileEntry();
                    entry.position = pos;
                    entry.tile = PrefabUtility.InstantiatePrefab(_tilePrefab,_tileParent.transform) as GameObject;
                    entry.tile.name = $"Tile({x},{z})";
                    entry.tile.transform.position = new Vector3(x, 0, z);
                    _tileManager.tileEntries.Add(entry);
                }
            }
        }
        else if(_curHeight < _prevHeight)
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
                    Debug.Log(pos+"감소");
                    foreach (var entry in _tileManager.tileEntries)
                    {
                        if (entry.position == pos)
                        {
                            Debug.Log(pos+"제거");
                            DestroyImmediate(entry.tile);
                            removeEntries.Add(entry);
                        }
                    }
                }
            }
            foreach (var entry in removeEntries)
            {
                _tileManager.tileEntries.Remove(entry);
                Debug.Log("리무브 엔트리?");
            }
        }
    }
}
