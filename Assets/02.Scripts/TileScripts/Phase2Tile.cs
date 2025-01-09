using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase2Tile : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    public int X = 0;
    public int Z = 0;
    void Start()
    {
        Phase2TileTileGrid();

    }
    private void Phase2TileTileGrid()
    {

        for (int x = 0; x <= X; x++)
        {
            for (int z = 0; z <= Z; z++)
            {
                Vector3 position = new Vector3(x , 0, z ); // 타일의 위치 계산
                Instantiate(tilePrefab, position, Quaternion.identity); // 타일 생성
            }
        }
    }
}
