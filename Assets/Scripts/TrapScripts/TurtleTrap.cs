using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleTrap : TrapTile
{
    public override void Use(GameObject player)
    {
        base.Use(player);
        Debug.Log("거북이 아이템 사용");
    }
}
