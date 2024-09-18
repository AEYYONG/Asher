using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleItem : Item
{
    public override void Use(GameObject player, Inventory inventory)
    {
        base.Use(player,inventory);
        Debug.Log("블랙홀 아이템 사용");
    }
}
