using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairBallItem : Tile
{
    public override void ItemUse(StageUIManager uiManager)
    {
        base.ItemUse(uiManager);
        Debug.Log("헤어볼 아이템 사용");
    }
}
