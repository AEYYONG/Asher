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

    void Awake()
    {
        //인벤토리 슬롯 할당
        firstSlot = transform.GetChild(0).GetChild(0).gameObject;
        secondSlot = transform.GetChild(1).GetChild(0).gameObject;
        
        //Slot 스크립트 할당
        slot1 = firstSlot.GetComponent<Slot>();
        slot2 = secondSlot.GetComponent<Slot>();
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

        tempID = slot2.GetID();
        tempImage = slot2.GetImage();
        
        slot2.SetID(slot1.GetID());
        slot2.SetImage(slot1.GetImage());
        
        slot1.SetID(tempID);
        slot1.SetImage(tempImage);
        
        //UI에 슬롯 정보 업데이터
        slot1.UpdateSlot();
        slot2.UpdateSlot();
    }

    public void AddItem(int slotNum,TileID id)
    {
        if (slotNum == 1)
        {
            slot1.SetID(id);
            isFirstSlotFull = true;
            slot1.UpdateSlot();
        }
        else if(slotNum == 2)
        {
            slot2.SetID(id);
            slot2.UpdateSlot();
        }
    }
}
