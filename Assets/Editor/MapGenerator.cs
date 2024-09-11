using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MapGenerator : EditorWindow
{
    private int _prevWidth = 0;
    private int _prevHeight = 0;
    
    //타일 생성
    private int _curWidth = 0; //타일 너비
    private int _curHeight = 0; //타일 높이
    private GameObject _tilePrefab; //타일 프리팹
    private GameObject _tileParent; //타일들이 생성될 부모 오브젝트
    
    //타일 딕셔너리
    private Dictionary<Vector2Int, GameObject> _tiles = new Dictionary<Vector2Int, GameObject>();
    
    
    //윈도우 메뉴에 "Map Generator"
    [MenuItem("Window/Map Generator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MapGenerator));
    }

    //actual window codes
    private void OnGUI()
    {
        GUILayout.Label("Tile Generation",EditorStyles.boldLabel);
        EditorGUILayout.Space();
        _curWidth = EditorGUILayout.IntSlider("width", _curWidth,0,20);
        _curHeight = EditorGUILayout.IntSlider("height", _curHeight,0,20);
        _tilePrefab = (GameObject)EditorGUILayout.ObjectField("tile", _tilePrefab, typeof(GameObject),false);
        _tileParent = (GameObject)EditorGUILayout.ObjectField("tile parent", _tileParent, typeof(GameObject), true);
        EditorGUILayout.Space();

        //타일의 width, height이 변화함에 따라 씬 내의 타일을 자동으로 배치
        if (_prevWidth != _curWidth)
        {
            Debug.Log("change width : "+_prevWidth+"->"+_curWidth);
            GenerateTile();
            _prevWidth = _curWidth;
        }
        if (_prevHeight != _curHeight)
        {
            Debug.Log("change height : "+_prevHeight+"->"+_curHeight);
            GenerateTile();
            _prevHeight = _curHeight;
        }

    }
    
    //씬 내 타일 배치
    void GenerateTile()
    {
        int tileCnt;
        int x;
        int z;
        
        //width 값이 증가했다면 가로로 타일 증가
        if (_prevWidth < _curWidth)
        {
            Debug.Log("타일의 width가 증가함.");
            //새로 생성되어야 하는 타일 개수 계산
            tileCnt = _curWidth - _prevWidth;
            for (int i = 0; i < tileCnt; i++)
            {
                x = _prevWidth + i;
                z = _curHeight;
                //타일 오브젝트 생성
                GameObject tile = Instantiate(_tilePrefab,new Vector3(x,0,z),_tilePrefab.transform.rotation);
                Debug.Log(x + " , " + z);
                //타일 부모 오브젝트의 자식 오브젝트로 설정
                tile.transform.SetParent(_tileParent.transform);
                //타일 오브젝트 이름 설정(Tile + TileID + Tile 좌표)
                tile.name = $"Tile({x},{z})";
                //타일 컴포넌트 가져와, 타일 정보 초기화
                tile.GetComponent<Tile>().InitTile(x,z);
                //타일 딕셔너리에 타일 오브젝트 추가
                _tiles.Add(new Vector2Int(x,z),tile);
            }
        }
        else //width 값이 변동이 없거나, 감소했다면 가로로 타일 감소
        {
            Debug.Log("타일의 width가 감소함.");  
            //감소해야 하는 타일 개수 계산
            tileCnt = _prevWidth - _curWidth;
            
            for (int i = 0; i < tileCnt; i++)
            {
                x = _curWidth - i;
                z = _curHeight;
                //타일 딕셔너리에서 타일을 찾아, 타일 제거
                Debug.Log(x + " , " + z);
                Vector2Int tilePos = new Vector2Int(x, z);
                DestroyImmediate(_tiles[tilePos]);
                _tiles.Remove(tilePos);
            }
            
        }
    }
}
