using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealBtn : MonoBehaviour
{

    // isPlayerTurn 변수 false로 변경, isNPCTurn false로 변경

    [SerializeField] private Phase2PlayerInteract Phase2PlayerInteract;
   // [SerializeField] private GameObject StealUI;
    [SerializeField] private Balloon balloon;
    [SerializeField] private Balloon balloon_npc;

    public void OnclickBtn()
    {
        // 변수 제어
        Phase2PlayerInteract.isPlayerTurn = false;
        Phase2PlayerInteract.isNPCTurn = false;
        balloon.isPlayer = 1;
        balloon.Condition = "steal";

        // UI 활성화
        //  StealUI.SetActive(true);
        gameObject.SetActive(false);

    }
}
