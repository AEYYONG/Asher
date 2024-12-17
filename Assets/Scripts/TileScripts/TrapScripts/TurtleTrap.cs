using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("거북이 함정 발동");
        StartCoroutine(SpeedDown(uiManager));
    }


    IEnumerator SpeedDown(StageUIManager uiManager)
    {
        VFXManager.Instance.PlayVFX("UseDebuffItem",uiManager.player.transform);
        yield return new WaitForSeconds(1.5f);
        
        Player_Move playerMove = uiManager.player.GetComponent<Player_Move>();
        playerMove.moveDuration *= tileSO.power;

        yield return new WaitForSeconds(tileSO.duration);
        Debug.Log("거북이 함정 지속 시간 끝");
        playerMove.moveDuration *= 1/tileSO.power;
        
        //vfx 실행
        Animator effectAnimator = transform.GetChild(0).GetComponent<Animator>();
        effectAnimator.SetTrigger("TrapMatch");
    }
    
}
