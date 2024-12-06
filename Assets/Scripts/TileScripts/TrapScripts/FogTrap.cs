using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("안개 아이템 사용");
    }

    IEnumerator SetFogSight()
    {
        yield return new WaitForSeconds(tileSO.duration);
        //vfx 실행
        Animator effectAnimator = transform.GetChild(0).GetComponent<Animator>();
        effectAnimator.SetTrigger("TrapMatch");
    }
}
