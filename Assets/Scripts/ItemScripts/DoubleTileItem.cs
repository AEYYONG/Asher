using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTileItem : ItemTile
{
    public override void Use(GameObject player)
    {
        base.Use(player);
        Debug.Log("이중 타일 아이템 사용");
    }
}
