using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("거북이 아이템 사용");
    }
}
