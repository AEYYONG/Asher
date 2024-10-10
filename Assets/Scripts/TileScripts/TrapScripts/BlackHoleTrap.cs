using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("블랙홀 아이템 사용");
    }
}
