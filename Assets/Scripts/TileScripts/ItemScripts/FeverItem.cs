using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverItem : Tile
{
    public override void ItemUse(StageUIManager uiManager)
    {
        base.ItemUse(uiManager);
        Debug.Log("피버타임 아이템 사용");
    }
}
