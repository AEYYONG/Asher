using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("역방향 아이템 사용");
    }
}
