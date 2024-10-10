using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

//한 스테이지에 사용할 타일 종류와 개수 구조체
[System.Serializable]
public class TileTypeStruct
{
    //타일 프리팹
    public GameObject tilePrefab;
    //타일 개수
    public int count;
}

[System.Serializable]
public class TileEntry
{
    public Vector2Int position;
    public GameObject tile;
}
public class TileManager : MonoBehaviour
{

    public List<Tile> allTiles;

    public List<Tile> GetAllTiles()
    {
        return allTiles;
    }

    // 특정 좌표의 타일을 가져오는 메서드
    public Tile GetTile(int x, int z)
    {
        return allTiles.Find(t => t._x == x && t._z == z);
    }


    //타일 가로, 세로 개수
    public int width, height;
    //타일이 다시 뒤집어지는 시간
    [SerializeField] public float tileReturnTime;
    
    //카메라
    [SerializeField] Camera cam;
    //타일 생성될 parent 오브젝트
    public GameObject tileParent;
    
    
    //타일 종류(프리팹, 개수) 리스트
    public List<TileTypeStruct> buffItemTypes = new List<TileTypeStruct>();
    public List<TileTypeStruct> debuffItemTypes = new List<TileTypeStruct>();
    public List<TileTypeStruct> etcItemTypes = new List<TileTypeStruct>();
    
    //타일 셔플을 위한 임시 리스트
    private List<GameObject> _tileShuffleList = new List<GameObject>();
    //타일 딕셔너리
    public Dictionary<Vector2Int, GameObject> _tiles = new Dictionary<Vector2Int, GameObject>(); //실제 사용 딕셔너리
    public List<TileEntry> tileEntries = new List<TileEntry>(); //직렬화 가능한 리스트
    
    //이벤트 타일 프리팹
    private GameObject _eventPrefab;
    public int eventTileCnt = 0;
    
    void Start()
    {
        _eventPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tiles/Etc/EventTile.prefab");
        _tiles.Clear();
        Debug.Log(_tiles.Count);
        InitTileList();
    }
    
    
    //타일 종류와 빈도에 따라 Tiles 딕셔너리에
    void InitTileList()
    {
        //타일 리스트에 타일 종류 순서 및 개수대로 넣기
        for (int type = 0; type < etcItemTypes.Count; type++)
        {
            for (int count = 0; count < etcItemTypes[type].count; count++)
            {
                _tileShuffleList.Add(etcItemTypes[type].tilePrefab);
            }
        }
        for (int type = 0; type < buffItemTypes.Count; type++)
        {
            for (int count = 0; count < buffItemTypes[type].count; count++)
            {
                _tileShuffleList.Add(buffItemTypes[type].tilePrefab);
            }
        }
        for (int type = 0; type < debuffItemTypes.Count; type++)
        {
            for (int count = 0; count < debuffItemTypes[type].count; count++)
            {
                _tileShuffleList.Add(debuffItemTypes[type].tilePrefab);
            }
        }
        //타일 리스트 섞기
        ShuffleTileList(_tileShuffleList);
    }

    //타일 리스트 섞는 함수
    //Fisher-Yates Shuffle 알고리즘
    void ShuffleTileList(List<GameObject> tileList)
    {
        for (int i = tileList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (tileList[i], tileList[randomIndex]) = (tileList[randomIndex], tileList[i]);
        }
        
        //셔플 완료된 타일 리스트를 보드에 배치하기
        ArrangeTilesOnBoard(tileList);
    }

    //보드에 타일 배치하기
    void ArrangeTilesOnBoard(List<GameObject> tileList)
    {
        int flag = 0;
        //맵에 배치된 타일이 저장된 tileEntries 리스트에서 타일의 설정된 아이디에 따라 프리팹, 태그 변경
        foreach (var entry in tileEntries)
        { 
            Tile tile = entry.tile.GetComponent<Tile>();
            Vector2Int pos = tile.ReturnPos();
            Debug.Log(tileList.Count);
            
            //타일 타입이 1(타일 배치 가능)
            if (tile.tileType == TileType.RandomAvail)
            {
                //Debug.Log("타입 1 " + entry.tile);
                Destroy(entry.tile);
                GameObject prefab = tileList[flag++];
                GameObject newTile = Instantiate(prefab, new Vector3(pos.x, 0, pos.y),
                    prefab.transform.rotation);
                newTile.transform.SetParent(tileParent.transform);
                newTile.GetComponent<Tile>().InitTile(pos.x,pos.y);
                _tiles.Add(pos,newTile);
                //타일 오브젝트 이름 설정(Tile + TileID + Tile 좌표)
                newTile.name = $"Tile{pos} : {newTile.name.Substring(0,newTile.name.Length - 7)}";
                
            }
            else if (tile.tileType == TileType.RandomNotAvail)
            {
                //Debug.Log("타입 2" + entry.tile);
                tile.name = $"Tile{pos} : Furniture Tile";
            }
            else if (tile.tileType == TileType.Event)
            {
                //추가적으로 이벤트 타일 문 확인 작성해줘야 함.
                //Debug.Log("타입 3 " + entry.tile);
                eventTileCnt++;
                Destroy(entry.tile);
                GameObject newTile = Instantiate(_eventPrefab, new Vector3(pos.x, 0, pos.y),
                    _eventPrefab.transform.rotation);
                newTile.transform.SetParent(tileParent.transform);
                newTile.GetComponent<Tile>().InitTile(pos.x,pos.y);
                _tiles.Add(pos,newTile);
                //타일 오브젝트 이름 설정(Tile + TileID + Tile 좌표)
                newTile.name = $"Tile{pos} : {newTile.name.Substring(0,newTile.name.Length - 7)}";
            }
        }
        //타일 보드가 중앙에 오도록 카메라 위치 조정
        //cam.transform.position = new Vector3((float)width / 2 - 0.5f, 10f,-((float)height / 2 - 1.7f));
    }

    public void ReturnTile(List<Tile> tiles)
    {
        foreach (var tile in tiles)
        {
            StartCoroutine(tile.ReturnTile());
        }
    }
}
