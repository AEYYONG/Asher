using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartStoneTile : Tile
{
    public override void Use()
    {
        StageUIManager stageUIManager = FindObjectOfType<StageUIManager>();
        stageUIManager.UpdateHeartStoneUI();
    }
}
