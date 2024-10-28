using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartStoneTile : Tile
{
    public override void Use(StageUIManager uiManager)
    {
        //base.Use(uiManager);
        uiManager.UpdateHeartStoneUI();
    }
}
