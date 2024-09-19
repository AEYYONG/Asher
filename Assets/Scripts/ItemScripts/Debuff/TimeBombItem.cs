using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBombTile : Item
{
    public override void Use(GameObject player, Inventory inventory)
    {
        base.Use(player,inventory);
        Debug.Log("시한폭탄 아이템 사용");
    }
}
