using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenZone_Vignette : MonoBehaviour
{


    // 비네트 프리팹(plane)
    public GameObject Plane;

    public int X = 0;
    public int Z = 0;

    // 그린존 위치 받아와야 함 어디서 호출되는건지 확인


    private List<GameObject> allTiles = new List<GameObject>(); // 모든 타일 리스트
    private List<GameObject> greenZoneTiles = new List<GameObject>(); // 그린존 타일 리스트

    void Start()
    {
        GenerateTileGrid(); // 타일 생성
        SetGreenZoneTiles();
        ApplyVignetteToNonGreenZoneTiles(); // 비네트 효과 추가
    }

    private void GenerateTileGrid()
    {
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Z; z++)
            {
                Vector3 position = new Vector3(x, 0.06f, z);
                if (IsGreenZonePosition(position))
                {
                    Debug.Log($"그린존 위치 타일 생성 제외: {position}");
                    continue; // 그린존 위치는 타일을 생성하지 않음
                }
                GameObject plane = Instantiate(Plane, position, Quaternion.identity);
                plane.name = $"Plane_{x}_{z}";
                allTiles.Add(plane); 
            }
        }
    }
    private bool IsGreenZonePosition(Vector3 position)
    {
        foreach (GameObject greenTile in greenZoneTiles)
        {
            if (greenTile.transform.position == position)
            {
                return true;
            }
        }
        return false;
    }

    private void SetGreenZoneTiles()
    {
        // 그린존 타일을 랜덤으로 선택
        for (int i = 0; i < 2; i++)
        {
            GameObject randomTile;

            // 중복되지 않도록 타일을 선택
            do
            {
                int randomIndex = Random.Range(0, allTiles.Count);
                randomTile = allTiles[randomIndex];
            } while (greenZoneTiles.Contains(randomTile)); // 이미 선택된 타일이면 다시 선택

            greenZoneTiles.Add(randomTile); // 선택된 타일을 그린존 리스트에 추가
            Debug.Log($"GreenZone {i + 1}: {randomTile.name}");
            Debug.Log(greenZoneTiles);
        }
        Debug.Log($"그린존 타일 개수: {greenZoneTiles.Count}");
        foreach (GameObject tile in greenZoneTiles)
        {
            Debug.Log(tile.name + " 위치: " + tile.transform.position);
        }
    }
    
    private void ApplyVignetteToNonGreenZoneTiles()
    {
        // 모든 타일 중 그린존 타일을 제외한 타일에 비네트 효과 추가
        foreach (GameObject tile in allTiles)
        {
            if (greenZoneTiles.Contains(tile))
            {
                tile.SetActive(false); // 그린존 타일 비활성화
                Debug.Log($"그린존 타일 비활성화: {tile.name}");
                continue; // 다음 타일로 넘어감
            }

            // 비네트를 적용해야 하는 타일
            AddVignetteOverlay(tile);
            Debug.Log($"비네트 적용: {tile.name}");

        }
    }


    private void AddVignetteOverlay(GameObject tile)
    {

        Renderer tileRenderer = tile.GetComponent<Renderer>();
        if (tileRenderer != null)
        {
            Color vignetteColor = new Color(0, 0, 0, 0.5f); // 반투명 검은색
            tileRenderer.material.color = vignetteColor;
        }
    }
}