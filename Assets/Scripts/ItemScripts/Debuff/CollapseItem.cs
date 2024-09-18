using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapseItem : Item
{
    public override void Use(GameObject player, Inventory inventory)
    {
        base.Use(player,inventory);
        Debug.Log("타일 붕괴 아이템 사용");
    }
}
