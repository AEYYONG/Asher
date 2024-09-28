using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseTrap : TrapTile
{
    public override void Use(GameObject player)
    {
        base.Use(player);
        Debug.Log("역방향 아이템 사용");
    }
}
