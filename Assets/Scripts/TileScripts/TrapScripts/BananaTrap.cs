using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("바나나 아이템 사용");
        Player_Move player = FindObjectOfType<Player_Move>();
        if (!player.isSlip)
        {
            player.StartSlip();
        }

    }
}
