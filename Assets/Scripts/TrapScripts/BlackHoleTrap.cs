using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleTrap : TrapTile
{
    public override void Use(GameObject player)
    {
        base.Use(player);
        Debug.Log("블랙홀 아이템 사용");
    }
}
