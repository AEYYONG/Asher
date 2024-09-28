using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrapID
{
    Banana,
    Turtle,
    Aggro,
    Fog,
    TileShuffle,
    Inverse,
    BlackHole,
    TimeBomb
}
[CreateAssetMenu(menuName = "TrapSO",fileName = "TrapSO")]
public class TrapSO : ScriptableObject
{
    public TrapID trapID;
}
