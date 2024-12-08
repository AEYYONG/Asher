using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("바나나 아이템 사용");
        StartCoroutine(StartSlip(uiManager));
    }

    IEnumerator StartSlip(StageUIManager uiManager)
    {
        VFXManager.Instance.PlayVFX("UseDebuffItem",uiManager.player.transform);
        yield return new WaitForSeconds(1.5f);
        Player_Move player = uiManager.player.GetComponent<Player_Move>();
        if (!player.isSlip)
        {
            player.StartSlip();
        }
        
        //vfx 실행
        Animator effectAnimator = transform.GetChild(0).GetComponent<Animator>();
        effectAnimator.SetTrigger("TrapMatch");
    }
}
