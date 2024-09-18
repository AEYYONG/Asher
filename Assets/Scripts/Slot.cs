using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    //아이템 아이디
    private TileID _tileID;
    //아이템 이미지
    private Sprite _itemImage;

    public void UpdateSlot()
    {
        GetComponent<Image>().sprite = _itemImage;
    }

    public TileID GetID()
    {
        return _tileID;
    }
    
    public void SetID(TileID id)
    {
        _tileID = id;
        foreach (var data in ItemData.instance.itemDatas)
        {
            if (data.id == id)
            {
                SetImage(data.img);
            }
        }
    }

    public Sprite GetImage()
    {
        return _itemImage;
    }
    
    public void SetImage(Sprite image)
    {
        _itemImage = image;
    }
}
