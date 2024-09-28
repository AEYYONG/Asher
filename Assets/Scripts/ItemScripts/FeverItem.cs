using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverItem : ItemTile
{
    public override void Use(GameObject player)
    {
        base.Use(player);
        Debug.Log("피버타임 아이템 사용");
    }
}
