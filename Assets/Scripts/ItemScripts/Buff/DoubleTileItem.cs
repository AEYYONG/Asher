using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTileItem : Item
{
    public override void Use(GameObject player, Inventory inventory)
    {
        base.Use(player,inventory);
        Debug.Log("이중 타일 아이템 사용");
    }
}
