using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Data
{
    public TileID id;
    public Sprite img;
}
public class ItemData : MonoBehaviour
{
    public static ItemData instance = null;
    public List<Data> itemDatas = new List<Data>();
    public TileID[] buffItems = { TileID.Memory, TileID.HairBall, TileID.Shield, TileID.DoubleTile, TileID.FeverTime };
    public TileID[] debuffItems =
    {
        TileID.Banana, TileID.Turtle, TileID.Aggro, TileID.Fog, TileID.Collapse, TileID.Inverse, TileID.BlackHole,
        TileID.TimeBomb
    };
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
