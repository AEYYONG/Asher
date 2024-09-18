using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryItem : Item
{
    public override void Use(GameObject player, Inventory inventory)
    {
        base.Use(player,inventory);
        Debug.Log("기억복원 아이템 사용");
    }
}
