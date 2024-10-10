using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDownTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("시간 감소 아이템 사용");
    }
}
