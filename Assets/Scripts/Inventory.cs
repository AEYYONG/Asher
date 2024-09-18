using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //인벤토리 내 두개의 슬롯
    public GameObject firstSlot;
    public GameObject secondSlot;
    private Slot slot1;
    private Slot slot2;
    
    //인벤토리 내 슬롯 채워져 있는지 여부
    public bool isFirstSlotFull = false;
    
    //ItemRunner 스크립트 참조
    private ItemRunner _itemRunner;
    
    void Awake()
    {
        //인벤토리 슬롯 할당
        firstSlot = transform.GetChild(0).GetChild(0).gameObject;
        secondSlot = transform.GetChild(1).GetChild(0).gameObject;
        
        //Slot 스크립트 할당
        slot1 = firstSlot.GetComponent<Slot>();
        slot2 = secondSlot.GetComponent<Slot>();
        
        //ItemRunner 스크립트 할당
        _itemRunner = FindObjectOfType<ItemRunner>();
    }

    void Update()
    {
        //아이템 Swap 버튼을 클릭한 경우
        if (Input.GetButtonUp("SwapItem"))
        {
            Debug.Log("아이템 Swap");
            SwapSlot();
        }
    }

    //인벤토리 슬롯1과 슬롯2의 위치 변경
    void SwapSlot()
    {
        //슬롯 값 교환
        TileID tempID;
        Sprite tempImage;
        Item tempScript;

        tempID = slot2.GetID();
        tempImage = slot2.GetImage();
        tempScript = slot2.GetScript();
        
        slot2.SetID(slot1.GetID());
        slot2.SetImage(slot1.GetImage());
        slot2.SetScript(slot1.GetScript());
        
        slot1.SetID(tempID);
        slot1.SetImage(tempImage);
        slot1.SetScript(tempScript);
        //현재 1번 슬롯에 위치한 아이템을 curItem으로 지정
        _itemRunner._curItem = slot1.GetScript();
        
        if (_itemRunner._curItem != null)
        {
            isFirstSlotFull = true;
        }
        else
        {
            isFirstSlotFull = false;
        }
        
        //UI에 슬롯 정보 업데이터
        slot1.UpdateSlot();
        slot2.UpdateSlot();
    }

    public void AddItem(int slotNum,TileInfo info)
    {
        if (slotNum == 1)
        {
            slot1.InitSlot(info);
            isFirstSlotFull = true;
            //현재 1번 슬롯에 위치한 아이템을 curItem으로 지정
            _itemRunner._curItem = slot1.GetScript();
            slot1.UpdateSlot();
        }
        else if(slotNum == 2)
        {
            slot2.InitSlot(info);
            slot2.UpdateSlot();
        }
    }
    
    //아이템 사용 시, 슬롯 비우기
    public void ClearItem()
    {
        slot1.ClearSlot();
        isFirstSlotFull = false;
        _itemRunner._curItem = null;
        slot1.UpdateSlot();
    }
}
