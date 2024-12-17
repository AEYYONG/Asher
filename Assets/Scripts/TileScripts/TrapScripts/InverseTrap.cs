using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        StartCoroutine(SetKeyInverse(uiManager));
    }

    IEnumerator SetKeyInverse(StageUIManager uiManager)
    {
        VFXManager.Instance.PlayVFX("UseDebuffItem",uiManager.player.transform);
        yield return new WaitForSeconds(1.5f);
        Debug.Log("역방향 함정 발동");
        Player_Move playerMove = uiManager.player.GetComponent<Player_Move>();
        playerMove.isInverse = true;
        yield return new WaitForSeconds(tileSO.duration);
        playerMove.isInverse = false;
        Debug.Log("역방향 함정 지속 끝");
        //vfx 실행
        Animator effectAnimator = transform.GetChild(0).GetComponent<Animator>();
        effectAnimator.SetTrigger("TrapMatch");
    }
}
