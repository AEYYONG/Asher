using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenZoneTile : Tile
{
    public override void Use(StageUIManager uiManager)
    {
        base.Use(uiManager);
        FindObjectOfType<NPC_Move>().isChasing = false;
        Debug.Log("그린존");
    }
}
