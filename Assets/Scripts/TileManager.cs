using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//한 스테이지에 사용할 타일 종류와 개수 구조체
[System.Serializable]
public struct TileTypeStruct
{
    //타일 프리팹
    public GameObject tilePrefab;
    //타일 개수
    public int count;
}
public class TileManager : MonoBehaviour
{
    //해당 스테이지의 타이머
    [SerializeField] private float timer;
    
    //타일 가로, 세로 개수
    [SerializeField] private int width, height;
    //타일 간격
    [SerializeField] private float tileSpacing = 1.0f;
    //타일이 다시 뒤집어지는 시간
    [SerializeField] public float tileReturnTime;
    
    //카메라
    [SerializeField] Camera cam;
    //타일 생성될 parent 오브젝트
    [SerializeField] private GameObject tileParent;
    
    
    //타일 종류(프리팹, 개수) 리스트
    [SerializeField] private List<TileTypeStruct> tileTypes = new List<TileTypeStruct>();
    //타일 셔플을 위한 임시 리스트
    private List<GameObject> _tileShuffleList = new List<GameObject>();
    //타일 딕셔너리
    private Dictionary<Vector2, GameObject> _tiles = new Dictionary<Vector2, GameObject>();
    
    void Awake()
    {
        CheckError();
    }

    void Start()
    {
        InitTileList();
    }

    //오류 검증
    void CheckError()
    {
        //필요 변수 할당하였는지
        if (width == 0 || height == 0 || tileSpacing == 0f || tileReturnTime == 0f)
        {
            Debug.Log("타일 너비/높이/간격/시간 이(가) 할당되지 않음");
        }
        //필요 오브젝트 할당하였는지
        if (cam == null || tileParent == null || tileTypes.Count == 0)
        {
            Debug.Log("오브젝트가 TileManager에 할당되지 않음");
        }
        //타일 수가 짝수인지(짝 맞춰야하니)
        for (int i = 0; i < tileTypes[i].count; i++)
        {
            if (tileTypes[i].count % 2 != 0)
            {
                Debug.Log("타일 짝이 맞지 않음");
            }
        }
        //각 타일에 설정한 할당량의 합이 총 타일 수와 맞는지
        int tileSum = 0;
        for (int i = 0; i < tileTypes.Count; i++)
        {
            tileSum += tileTypes[i].count;
        }
        if (tileSum != width * height)
        {
            Debug.Log("타일 프리팹 할당 타일 수가 부족함");
        }
        
    }
    //타일 종류와 빈도에 따라 Tiles 딕셔너리에
    void InitTileList()
    {
        //타일 리스트에 타일 종류 순서 및 개수대로 넣기
        for (int type = 0; type < tileTypes.Count; type++)
        {
            for (int count = 0; count < tileTypes[type].count; count++)
            {
                _tileShuffleList.Add(tileTypes[type].tilePrefab);
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
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                //타일 오브젝트 생성
                GameObject tile = Instantiate(tileList[x * height + z], new Vector3(x*tileSpacing,0,z*tileSpacing),
                    tileList[x * height + z].transform.rotation);
                //타일부모의 자식으로 오브젝트 설정
                tile.transform.SetParent(tileParent.transform);
                //타일 프리팹 이름 배열 값으로 가져오기
                string prefabName = tileList[x * height + z].ToString();
                //타일 오브젝트 이름 설정(Tile + TileID + Tile 좌표)
                tile.name = $"Tile{prefabName[4]}({x},{z})";
                //타일 컴포넌트 가져와, 타일 정보 초기화
                tile.GetComponent<Tile>().InitTile(x,z,prefabName);
                //타일 딕셔너리에 타일 오브젝트 추가
                _tiles.Add(new Vector2(x,z),tile);
            }
        }
        //타일 보드가 중앙에 오도록 카메라 위치 조정
        cam.transform.position = new Vector3((float)width / 2 - 0.5f, 10f,-((float)height / 2 - 1.7f));
    }

    public void ReturnTile(Vector2 pos1, Vector2 pos2)
    {
        StartCoroutine(_tiles[pos1].GetComponent<Tile>().ReturnTile());
        StartCoroutine(_tiles[pos2].GetComponent<Tile>().ReturnTile());
    }
}
