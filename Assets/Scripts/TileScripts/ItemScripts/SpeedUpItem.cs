using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpItem : Tile
{
    public override void ItemUse(StageUIManager uiManager)
    {
        base.ItemUse(uiManager);
        Debug.Log("보호막 아이템 사용");
    }
}
