using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroItem : Item
{
    public override void Use(GameObject player, Inventory inventory)
    {
        base.Use(player,inventory);
        Debug.Log("어그로 아이템 사용");
    }
}
