using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileID
{
    General,
    Event,
    Joker,
    Item,
    Trap
}
[CreateAssetMenu(menuName = "TileSO",fileName = "TileSO")]
public class TileSO : ScriptableObject
{
    public TileID tileID;
}
