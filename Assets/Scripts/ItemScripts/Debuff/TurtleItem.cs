using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleItem : Item
{
    public override void Use(GameObject player, Inventory inventory)
    {
        base.Use(player,inventory);
        Debug.Log("거북이 아이템 사용");
    }
}
