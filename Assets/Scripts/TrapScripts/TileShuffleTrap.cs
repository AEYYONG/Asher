using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileShuffleTrap : TrapTile
{
    public override void Use(GameObject player)
    {
        base.Use(player);
        Debug.Log("타일 붕괴 아이템 사용");
    }
}
