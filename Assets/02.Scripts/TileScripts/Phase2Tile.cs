using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase2Tile : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private List<GameObject> clickableTiles = new List<GameObject>(); // 클릭 가능한 타일 리스트
    [SerializeField] private List<Vector3> predefinedClickablePositions; // 클릭 가능한 타일 위치 리스트
    [SerializeField] private GameObject redDot;
    public GameObject npcObject;


    public int X = 0;
    public int Z = 0;
    // 타일 생성
    void Start()
    {
        Reposition();
        Phase2TileTileGrid();
        ApplyVignetteToNonClickableTiles();
        StartCoroutine(ShowRedDotsOnTiles());
    }
    private void Phase2TileTileGrid()
    {
        for (int x = 0; x <= X; x++)
        {
            for (int z = 0; z <= Z; z++)
            {
                Vector3 position = new Vector3(x, 0, z); // 타일 위치 계산
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity); // 타일 생성

                // 생성된 타일이 클릭 가능한 위치에 포함되는지 검사
                if (predefinedClickablePositions.Contains(position))
                {
                    clickableTiles.Add(tile); // 클릭 가능한 타일 리스트에 추가
                }
            }

        }
    }


   

    private void ApplyVignetteToNonClickableTiles()
    {
        GameObject[] allTiles = GameObject.FindGameObjectsWithTag("Phase2");
        foreach (GameObject tile in allTiles)
        {
            // 클릭 가능한 타일이면 비네트 추가하지 않음
            if (clickableTiles.Contains(tile)) continue;

            // 클릭 불가능한 타일에 반투명한 오버레이 추가
            AddVignetteOverlay(tile);
        }
    }

    private void AddVignetteOverlay(GameObject tile)
    {
        Debug.Log("비네트 추가");

        // Plane 생성
        GameObject vignetteOverlay = GameObject.CreatePrimitive(PrimitiveType.Plane); // Plane 생성
        vignetteOverlay.name = "VignetteOverlay";
        vignetteOverlay.transform.SetParent(tile.transform);
        vignetteOverlay.transform.localPosition = new Vector3(0, 0.6f, 0); // 타일 위에 약간 띄움
        vignetteOverlay.transform.localScale = new Vector3(0.1f, 1, 0.1f); // 타일 크기에 맞춤 (Plane 기본 크기 보정)

        // 새 머티리얼 생성
        Material vignetteMaterial = new Material(Shader.Find("UI/Unlit/Transparent")); // Unlit/Color 사용
        vignetteMaterial.color = new Color(0, 0, 0, 0.75f); // 검은색 반투명

        // Plane에 머티리얼 적용
        MeshRenderer renderer = vignetteOverlay.GetComponent<MeshRenderer>();
        renderer.material = vignetteMaterial;

        Collider collider = vignetteOverlay.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider); 
        }
    }
    private IEnumerator ShowRedDotsOnTiles()
    {
        // 클릭 가능한 타일 중 3개를 랜덤 선택
        List<GameObject> selectedTiles = new List<GameObject>();
        List<GameObject> availableTiles = new List<GameObject>(clickableTiles);
        

        for (int i = 0; i < 3; i++)
        {
            if (availableTiles.Count == 0) break;

            int randomIndex = Random.Range(0, availableTiles.Count);
            GameObject randomTile = availableTiles[randomIndex];
            selectedTiles.Add(randomTile);
            availableTiles.RemoveAt(randomIndex);
        }

        // 선택된 타일에 빨간 점 표시
        foreach (GameObject tile in selectedTiles)
        {
            redDot.transform.localPosition = new Vector3(0, 0.1f, 0); // 타일 위에 표시
            yield return new WaitForSeconds(1f); // 1초 대기
            Destroy(redDot); // 빨간 점 제거
        }

        Debug.Log("빨간 점 표시 완료");
    }

    // npc,플레이어 위치

    private void Reposition()
    {
        NPC_Move npc = npcObject.GetComponent<NPC_Move>();

        if (npc == null)
        {
            Debug.LogError("NPC_Move 오브젝트를 찾을 수 없습니다.");
            return; // 메서드 종료
        }

        if (npc.agent == null)
        {
            Debug.LogError("NavMeshAgent가 NPC_Move 오브젝트에 없습니다.");
            return; // 메서드 종료
        }

        npc.agent.isStopped = true;
        npc.transform.position = new Vector3(6, npc.transform.position.y, 6);
        Debug.Log("멈춤");
        Player_Move.Instance.transform.position = new Vector3(6, Player_Move.Instance.transform.position.y, 0.5f);
        Player_Move.Instance.isStart = false;
        Debug.Log("플레이어 멈춤");
    }

}