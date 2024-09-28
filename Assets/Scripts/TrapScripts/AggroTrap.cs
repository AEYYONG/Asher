using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroTrap : TrapTile
{
    public override void Use(GameObject player)
    {
        base.Use(player);
        Debug.Log("어그로 아이템 사용");
    }
}
