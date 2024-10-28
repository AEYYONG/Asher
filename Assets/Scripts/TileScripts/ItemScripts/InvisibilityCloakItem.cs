using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibilityCloakItem : Tile
{
    public override void ItemUse(StageUIManager uiManager)
    {
        base.ItemUse(uiManager);
        Debug.Log("투명망토 아이템 사용");
        StartCoroutine(SetInvisible(uiManager));
    }

    IEnumerator SetInvisible(StageUIManager uiManager)
    {
        uiManager.player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        uiManager.npc.GetComponent<NPC_Move>().canDetect = false;
        
        yield return new WaitForSeconds(tileSO.duration);
        
        uiManager.player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        uiManager.npc.GetComponent<NPC_Move>().canDetect = true;
    }
}
