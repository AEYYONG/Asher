using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public enum ItemType
{
    None,
    Memory,
    HairBall,
    DoubleTile,
    Fever,
    SpeedUp
}
[CreateAssetMenu(menuName = "ItemSO",fileName = "ItemSO")]
public class ItemSO : ScriptableObject
{
    public ItemType itemType;
    public Sprite img;
}
