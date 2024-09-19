using System;
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
    //아이템 스크립트
    private Item _script;
    //TileManager 스크립트 할당
    private TileManager _tileManager;

    void Awake()
    {
        //TileManager 스크립트 할당
        _tileManager = FindObjectOfType<TileManager>();
    }

    public void UpdateSlot()
    {
        GetComponent<Image>().sprite = _itemImage;
    }

    public TileID GetID()
    {
        return _tileID;
    }
    
    public void InitSlot(TileInfo info)
    {
        _tileID = info.tileID;
        foreach (var data in ItemData.instance.itemDatas)
        {
            if (data.id == _tileID)
            {
                SetImage(data.img);
            }
        }
        _script = _tileManager._tiles[info.tilePos].GetComponent<Item>();
    }
    
    public void SetID(TileID id)
    {
        _tileID = id;
    }

    public Sprite GetImage()
    {
        return _itemImage;
    }
    
    public void SetImage(Sprite image)
    {
        _itemImage = image;
    }
    
    public Item GetScript()
    {
        return _script;
    }
    
    public void SetScript(Item script)
    {
        _script = script;
    }

    public void ClearSlot()
    {
        _tileID = TileID.General;
        _script = null;
        foreach (var data in ItemData.instance.itemDatas)
        {
            if (data.id == _tileID)
            {
                _itemImage = data.img;
            }
        }
    }
}
