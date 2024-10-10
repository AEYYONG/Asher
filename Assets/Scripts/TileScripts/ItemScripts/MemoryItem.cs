using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryItem : Tile
{
    
    public override void ItemUse()
    {
        Debug.Log("기억복원 아이템 사용");
    }
}
