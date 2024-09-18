using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
