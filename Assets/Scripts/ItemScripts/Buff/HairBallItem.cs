using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairBallItem : Item
{
    public override void Use(GameObject player, Inventory inventory)
    {
        base.Use(player,inventory);
        Debug.Log("헤어볼 아이템 사용");
    }
}
