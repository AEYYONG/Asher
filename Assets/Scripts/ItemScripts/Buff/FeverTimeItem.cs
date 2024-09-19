using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverTimeItem : Item
{
    public override void Use(GameObject player, Inventory inventory)
    {
        base.Use(player,inventory);
        Debug.Log("피버타임 아이템 사용");
    }
}
