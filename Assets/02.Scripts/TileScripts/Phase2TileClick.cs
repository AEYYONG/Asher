using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase2TileClick : MonoBehaviour
{
    [SerializeField] private List<GameObject> clickableTiles = new List<GameObject>();

    [SerializeField] private GameObject redCirclePrefab;

    private List<Vector3> selectedPositions = new List<Vector3>();
    private List<Vector3> playerClickOrder = new List<Vector3>();
    private bool isClickEnabled = false;

   

    void Update()
    {
        if (isClickEnabled && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 clickedPosition = new Vector3(
                    Mathf.Round(hit.point.x),
                    0,
                    Mathf.Round(hit.point.z)
                );

                if (selectedPositions.Contains(clickedPosition))
                {
                    playerClickOrder.Add(clickedPosition);

                    if (playerClickOrder.Count == selectedPositions.Count)
                    {
                        bool isCorrect = true;
                        for (int i = 0; i < selectedPositions.Count; i++)
                        {
                            if (playerClickOrder[i] != selectedPositions[i])
                            {
                                isCorrect = false;
                                break;
                            }
                        }

                        if (isCorrect)
                        {
                            Debug.Log("클리어!");
                        }
                        else
                        {
                            Debug.Log("실패. 올바른 순서로 클릭하지 않았습니다.");
                        }

                        // 상태 초기화
                        playerClickOrder.Clear();
                        selectedPositions.Clear();
                        isClickEnabled = false;

                        // 다시 시작
                    
                    }
                }
                else
                {
                    Debug.Log("잘못된 타일을 클릭했습니다.");
                }
            }
        }
    }
}
