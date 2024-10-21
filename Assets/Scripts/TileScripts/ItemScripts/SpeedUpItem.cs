using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpItem : Tile
{
    public override void ItemUse(StageUIManager uiManager)
    {
        base.ItemUse(uiManager);
        Debug.Log("속도증가 아이템 사용");
        StartCoroutine(SpeedUp(uiManager));
    }

    IEnumerator SpeedUp(StageUIManager uiManager)
    {
        Player_Move_anim playerMove = uiManager.player.GetComponent<Player_Move_anim>();
        playerMove.moveDuration *= 1/tileSO.power;
        
        yield return new WaitForSeconds(tileSO.duration);
        Debug.Log("속도증가 아이템 지속 시간 끝");
        playerMove.moveDuration *= tileSO.power;
    }
}
