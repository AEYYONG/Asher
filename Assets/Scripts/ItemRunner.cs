using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//아이템 추상 클래스
public abstract class Item : MonoBehaviour
{
    //아이템 사용 가상 함수(자식 클래스에서 재정의 가능하도록)
    public virtual void Use(GameObject player, Inventory inventory)
    {
        
    }
}
public class ItemRunner : MonoBehaviour
{
    //현재 아이템
    public Item _curItem;
    //인벤토리 컴포넌트 참조
    private Inventory _inventory;

    void Awake()
    {
        _inventory = FindObjectOfType<Inventory>();
    }

    void Update()
    {
        //아이템 사용 버튼을 클릭하였고 현재 선택된 아이템이 존재하고 아이템을 사용 중이지 않은 경우
        if (Input.GetButtonUp("UseItem") && _curItem != null)
        {
            _curItem.Use(this.gameObject,_inventory);
            //인벤토리 1번 슬롯 삭제
            _inventory.ClearItem();
        }
    }
}
