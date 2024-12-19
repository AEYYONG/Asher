using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("안개 아이템 사용");
        StartCoroutine(SetFogSight(uiManager));
    }

    IEnumerator SetFogSight(StageUIManager uiManager)
    {
        //카메라 변경
        uiManager.fullCamera.SetActive(false);
        uiManager.npcIndicator.SetActive(true);
        uiManager.fogUI.SetActive(true);
        yield return new WaitForSeconds(tileSO.duration);
        uiManager.fullCamera.SetActive(true);
        uiManager.npcIndicator.SetActive(false);
        uiManager.fogUI.SetActive(false);
        
        //vfx 실행
        Animator effectAnimator = transform.GetChild(0).GetComponent<Animator>();
        effectAnimator.SetTrigger("TrapMatch");
    }
}
