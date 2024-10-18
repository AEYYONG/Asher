using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTileItem : Tile
{
    public override void ItemUse(StageUIManager uiManager)
    {
        base.ItemUse(uiManager);
        Debug.Log("이중타일 아이템 사용");
    }
}
