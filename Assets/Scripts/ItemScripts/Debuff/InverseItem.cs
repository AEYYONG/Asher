using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseItem : Item
{
    public override void Use(GameObject player, Inventory inventory)
    {
        base.Use(player,inventory);
        Debug.Log("역방향 아이템 사용");
    }
}
