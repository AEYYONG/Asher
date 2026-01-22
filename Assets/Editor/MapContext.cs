using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum DrawMode
{
    DEFAULT,
    BRUSH,
    ERASER,
    SELECT,
    DISSELECT
};

public enum TypeMode
{
    DEFAULT,
    AVAIL,
    NOTAVAIL,
    EVENT
};

public struct TexEntry
{
    public Vector2Int pos;
    public GameObject tex;
};
public class MapContext
{
    public TileManager tileManager;
    public readonly string prefabPath = "Assets/04.Prefabs/CustomEditor/";
    
    // Grid 생성
    public int gridWidth; //grid 가로 개수
    public int gridHeight; //grid 세로 개수 
    public GameObject gridPrefab;
    public GameObject gridParent; //생성된 grid들이 들어갈 부모 오브젝트
    public List<GameObject> gridList = new List<GameObject>(); //grid 오브젝트들을 저장할 리스트
    
    // 타일 생성
    public int prevWidth; //이전 타일 너비
    public int prevHeight; //이전 타일 높이
    public int curWidth; //타일 너비
    public int curHeight; //타일 높이
    public GameObject tilePrefab; //타일 프리팹
    public GameObject tileParent; //타일들이 생성될 부모 오브젝트
    
    // 타일 그리기
    public DrawMode curDrawMode;
    public GameObject selectedTile;
    public TypeMode curTypeMode;
    public GameObject notAvailTile;
    public GameObject eventTile;
    public List<TexEntry> texList = new List<TexEntry>();
    public GameObject texParent;
    
    //타일 종류
    public int totalTileCnt;
    public bool buffItemStatus;
    public bool debuffItemStatus;
    public bool etcItemStatus;
    public List<TileTypeStruct> curTileTypeList;
}
