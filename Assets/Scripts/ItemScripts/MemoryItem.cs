using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryItem : ItemTile
{
    
    public override void Use(GameObject player)
    {
        base.Use(player);
        Debug.Log("기억복원 아이템 사용");
    }
}
