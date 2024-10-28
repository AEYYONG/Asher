using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverItem : Tile
{
    public override void ItemUse(StageUIManager uiManager)
    {
        base.ItemUse(uiManager);
        Debug.Log("피버타임 아이템 사용");
        StartCoroutine(FeverTime(uiManager));
    }

    IEnumerator FeverTime(StageUIManager uiManager)
    {
        uiManager.player.GetComponent<PlayerInteract>().isFever = true;
        Player_Move_anim playerMove = uiManager.player.GetComponent<Player_Move_anim>();
        playerMove.moveDuration *= 1/tileSO.power;

        yield return new WaitForSeconds(tileSO.duration);
        
        uiManager.player.GetComponent<PlayerInteract>().isFever = false;
        playerMove.moveDuration *= tileSO.power;
    }
}
