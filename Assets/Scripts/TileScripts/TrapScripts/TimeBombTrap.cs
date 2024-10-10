using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBombTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("시한폭탄 아이템 사용");
    }
}
