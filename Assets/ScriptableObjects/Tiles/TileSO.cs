using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    MemoryGlass,
    HairBall,
    SpeedUp,
    InvisibilityCloak,
    FeverTime
}

public enum TrapID
{
    Default,
    Aggro,
    Banana,
    BlackHole,
    Fog,
    Inverse,
    TimeBomb,
    Gauntlet,
    Turtle,
}

[CreateAssetMenu(menuName = "TileSO",fileName = "TileSO")]
public class TileSO : ScriptableObject
{
    public TileID tileID;
    public ItemID itemID;
    public JokerID jokerID;
    public TrapID trapID;
    public Sprite itemImg;
    public Sprite sideCutSceneImg;
    public int selectNum;
    //지속시간
    public float duration;
    //파워
    public float power;
    //투명 고글 아이템 -> 투명 버전 타일 텍스쳐
    public Texture2D originTopTex;
    public Texture2D transTopTex;
    //시한폭탄 함정 -> 빛나는 텍스쳐
    public Texture2D timeBombTopTex;
}
