using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogItem : Item
{
    public override void Use(GameObject player, Inventory inventory)
    {
        base.Use(player,inventory);
        Debug.Log("안개 아이템 사용");
    }
}
