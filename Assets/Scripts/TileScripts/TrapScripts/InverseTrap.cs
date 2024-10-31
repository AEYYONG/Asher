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
        Debug.Log("역방향 함정 발동");
        Player_Move_anim playerMove = uiManager.player.GetComponent<Player_Move_anim>();
        playerMove.isInverse = true;
        yield return new WaitForSeconds(tileSO.duration);
        playerMove.isInverse = false;
        Debug.Log("역방향 함정 지속 끝");
    }
}
