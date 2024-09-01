using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TileTypeStruct
{
    public GameObject tilePrefab;
    public int count;
}
public class TileManager : MonoBehaviour
{
    //타일 가로, 세로 개수
    [SerializeField] private int width, height;
    //타일 간격
    [SerializeField] private float tileSpacing = 1.0f;
    //카메라
    [SerializeField] Camera cam;
    //타일 생성될 parent 오브젝트
    [SerializeField] private GameObject tileParent;
    //타일 종류(프리팹, 개수) 리스트
    [SerializeField] private List<TileTypeStruct> tileTypes = new List<TileTypeStruct>();
    //타일 딕셔너리
    public Dictionary<Vector3, GameObject> tiles = new Dictionary<Vector3, GameObject>();
    public GameObject tilePrefab;


    void Start()
    {
        CreateTile();
    }

    void CreateTile()
    {
        cam.transform.position = new Vector3((float)width / 2 - 0.5f, 10f,-((float)height / 2 - 1.7f));
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 tilePos = new Vector3(x * tileSpacing, 0 , z * tileSpacing);
                GameObject tile = Instantiate(tilePrefab, new Vector3(x,0,z),
                    tilePrefab.transform.rotation);
                tile.transform.SetParent(tileParent.transform);
                tile.name = $"Tile({x},{z})";
                tiles.Add(tilePos,tile);
            }
        }

    }
}
