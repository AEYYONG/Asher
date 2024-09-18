using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaItem : Item
{
    public override void Use(GameObject player, Inventory inventory)
    {
        base.Use(player,inventory);
        Debug.Log("바나나 아이템 사용");
    }
}
