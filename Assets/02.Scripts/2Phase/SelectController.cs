using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectController : MonoBehaviour
{
    [SerializeField] private GameObject resultUI;

    [SerializeField] private Transform npcPitchParent; // NPCPitch 부모 오브젝트
    [SerializeField] private GameObject choosePointUI;

    [SerializeField] private Phase2PlayerInteract Phase2PlayerInteract;

    private int selectedIndex = 0;
    private List<Vector3> npcPositions = new List<Vector3>();

    void Start()
    {
        resultUI.SetActive(false);
        InitializeSelection();
    }

    void Update()
    {
        HandleSelectionInput();
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
           
            Debug.Log($"dnlfmf snfma 인덱스 변경 (↑): {selectedIndex}");

        }
    }

    public void ActivateSelection()
    {
        InitializeSelection();
        if (npcPositions.Count > 0)
        {
            selectedIndex = 0; // 첫 번째 항목 선택
            choosePointUI.SetActive(true); 
            UpdateChoosePointPosition();
        }
    }

    private void InitializeSelection()
    {
        npcPositions.Clear();
        foreach (Transform child in npcPitchParent)
        {
            if (child.gameObject.activeSelf)
            {
                npcPositions.Add(child.position); // 위치만 리스트에 추가
            }
        }

        if (npcPositions.Count == 0)
        {
            choosePointUI.SetActive(false); // 선택할 게 없으면 숨김
        }
    }

    private void HandleSelectionInput()
    {
        if (npcPositions.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedIndex = (selectedIndex - 1 + npcPositions.Count) % npcPositions.Count;
            Debug.Log($"선택된 인덱스 변경 (↑): {selectedIndex}");
            UpdateChoosePointPosition();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedIndex = (selectedIndex + 1) % npcPositions.Count;
            Debug.Log($"선택된 인덱스 변경 (↓): {selectedIndex}");
            UpdateChoosePointPosition();
        }
        else if (Input.GetKeyDown(KeyCode.Return)) // 엔터 누르면 확정
        {
            SelectCurrentObject();
        }
    }

    private void UpdateChoosePointPosition()
    {
        if (choosePointUI != null && npcPositions.Count > 0)
        {
            Vector3 newPosition = npcPositions[selectedIndex] + new Vector3(-30f, -110f, 0);
            choosePointUI.transform.position = newPosition;
            Debug.Log($"포인터 이동: {choosePointUI.transform.position}");
        }
    }

    private void SelectCurrentObject()
    {
        Debug.Log($"선택된 위치: {npcPositions[selectedIndex]} (인덱스: {selectedIndex})");
        Debug.Log($" Phase2PlayerInteract.isSteal 값 변경: true");
        Debug.Log($" Phase2PlayerInteract.isPlayerTurn 값 변경: true");
        // 선택 UI 숨기기

    
        Phase2PlayerInteract.SetStealIndex(selectedIndex);
        Phase2PlayerInteract.isSteal = true;
        Phase2PlayerInteract.isStealDone = true;
        Debug.Log(" SelectController 비활성화 실행됨");

        gameObject.SetActive(false); 
    }
}
