using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaTrap : TrapTile
{
    public override void Use(GameObject player)
    {
        base.Use(player);
        Debug.Log("바나나 아이템 사용");
    }
}
