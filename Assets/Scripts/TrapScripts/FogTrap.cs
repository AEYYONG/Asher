using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogTrap : TrapTile
{
    public override void Use(GameObject player)
    {
        base.Use(player);
        Debug.Log("안개 아이템 사용");
    }
}
