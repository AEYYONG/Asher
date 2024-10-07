using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileID
{
    General,
    Event,
    Joker,
    Item,
    Trap,
    GreenZone,
    HeartStone,
}

public enum JokerID
{
    Default,
    RedJoker,
    BlackJoker
}

public enum ItemID
{
    Default,
    Memory,
    HairBall,
    SpeedUp,
    DoubleTile,
    FeverTime
}

public enum TrapID
{
    Default,
    Banana,
    Turtle,
    Aggro,
    Fog,
    TimeDown,
    Inverse,
    BlackHole,
    TimeBomb
}
[CreateAssetMenu(menuName = "TileSO",fileName = "TileSO")]
public class TileSO : ScriptableObject
{
    public TileID tileID;
    public ItemID itemID;
    public JokerID jokerID;
    public TrapID trapID;
}
