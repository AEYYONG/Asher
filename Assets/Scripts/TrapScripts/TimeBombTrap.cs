using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBombTrap : TrapTile
{
    public override void Use(GameObject player)
    {
        base.Use(player);
        Debug.Log("시한폭탄 아이템 사용");
    }
}
