using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("어그로 아이템 사용");
    }
}
