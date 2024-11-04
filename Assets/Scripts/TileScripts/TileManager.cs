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
    
    //가구 타일의 위치
    public List<Vector2Int> furnitureTilePosList = new List<Vector2Int>();
    
    //타일 셔플을 위한 임시 리스트
    public List<GameObject> _tileShuffleList = new List<GameObject>();
    //타일 딕셔너리
    public Dictionary<Vector2Int, GameObject> _tiles = new Dictionary<Vector2Int, GameObject>(); //실제 사용 딕셔너리
    public List<TileEntry> tileEntries = new List<TileEntry>(); //직렬화 가능한 리스트
    
    //그린존 타일 리스트
    public List<GameObject> greenZoneTileList = new List<GameObject>();
    public bool isGreenZoneActive = false;
    private NPC_Move _npcMove; //적 npc
    private PlayerInteract _playerInteract; //플레이어 상호작용
    
    void Start()
    {
        _tiles.Clear();
        Debug.Log(_tiles.Count);
        InitTileList();
        
        //NPC 할당
        _npcMove = GameObject.FindWithTag("NPC").GetComponent<NPC_Move>();
        //PlayerInteract 할당
        _playerInteract = GameObject.FindWithTag("Player").GetComponent<PlayerInteract>();
    }

    void Update()
    {
        if (!isGreenZoneActive && _npcMove.isChasing && !_playerInteract.useGreenZone)
        {
            ActiveGreenZone();
        }
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
        //셔플 완료된 타일 리스트를 보드에 배치하기
        ArrangeTilesOnBoard(ShuffleTileList(_tileShuffleList));
    }

    //타일 리스트 섞는 함수
    //Fisher-Yates Shuffle 알고리즘
    public List<GameObject> ShuffleTileList(List<GameObject> tileList)
    {
        for (int i = tileList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (tileList[i], tileList[randomIndex]) = (tileList[randomIndex], tileList[i]);
        }

        return tileList;
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
                Tile newTileScript = newTile.GetComponent<Tile>();
                newTileScript.InitTile(pos.x,pos.y);
                _tiles.Add(pos,newTile);
                //타일 오브젝트 이름 설정(Tile + TileID + Tile 좌표)
                newTile.name = $"Tile{pos} : {newTile.name.Substring(0,newTile.name.Length - 7)}";
                
                //그린존 타일일 경우, 그린존 딕셔너리에서 추가적으로 관리
                if (newTileScript.tileSO.tileID == TileID.GreenZone)
                {
                    greenZoneTileList.Add(newTile);
                }
                
            }
            else if (tile.tileType == TileType.RandomNotAvail)
            {
                Vector2Int furnitureTilePos = tile.ReturnPos();
                furnitureTilePosList.Add(furnitureTilePos);
                //Debug.Log("타입 2" + entry.tile);
                tile.name = $"Tile{pos} : Furniture Tile";
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

    // 추격 상황 시, 그린존 타일을 활성화
    public void ActiveGreenZone()
    {
        foreach (var tile in greenZoneTileList)
        {
            Tile script = tile.GetComponent<Tile>();
            //선택 여부 true로 변경
            script.isSelected = true;
            //뒤집기 애니메이션 시작
            script._animator.SetTrigger("Select");
        }
        isGreenZoneActive = true;
    }

    //그린존 비활성화
    public void InActiveGreenZone()
    {
        foreach (var tile in greenZoneTileList)
        {
            Tile script = tile.GetComponent<Tile>();
            StartCoroutine(script.ReturnTile());
        }
        isGreenZoneActive = false;
    }
    
}
