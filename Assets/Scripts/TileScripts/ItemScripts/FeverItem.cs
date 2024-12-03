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
        SetRainbowMaterial(uiManager,true);
        uiManager.player.GetComponent<PlayerInteract>().isFever = true;
        Player_Move playerMove = uiManager.player.GetComponent<Player_Move>();
        playerMove.moveDuration *= 1/tileSO.power;

        yield return new WaitForSeconds(tileSO.duration);

        SetRainbowMaterial(uiManager, false);
        uiManager.player.GetComponent<PlayerInteract>().isFever = false;
        playerMove.moveDuration *= tileSO.power;
    }

    void SetRainbowMaterial(StageUIManager uiManager,bool isEnabled)
    {
        //애셔가 갖고 있는 머티리얼 가져와서 레인'
        Renderer renderer = uiManager.player.GetComponent<Renderer>();
        int value = isEnabled ? 1 : 0;
        renderer.material.SetFloat("_EnableRainbow", value);
        renderer.material = new Material(renderer.material);
    }
}
