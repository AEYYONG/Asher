using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealBtn : MonoBehaviour
{

    // isPlayerTurn 변수 false로 변경, isNPCTurn false로 변경

    [SerializeField] private Phase2PlayerInteract Phase2PlayerInteract;
    [SerializeField] private GameObject StealUI;

    public void OnclickBtn()
    {
        // 변수 제어
        Phase2PlayerInteract.isPlayerTurn = false;
        Phase2PlayerInteract.isNPCTurn = false;
        // UI 활성화
        StealUI.SetActive(true);

    }
}
