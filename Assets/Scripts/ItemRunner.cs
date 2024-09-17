using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//아이템 추상 클래스
public abstract class Item : ScriptableObject
{
    [SerializeField] protected string itemName;

    //아이템 사용 가상 함수(자식 클래스에서 재정의 가능하도록)
    public virtual void Use(GameObject player)
    {
        Debug.Log($"아이템 {itemName} 사용");
    }
}
public class ItemRunner : MonoBehaviour
{
    //현재 아이템
    private Item _curItem;
    //아이템을 사용 중인지
    private bool _isRunning;

    void Awake()
    {
        _isRunning = false;
    }

    void Update()
    {
        //아이템 사용 버튼을 클릭하였고 현재 선택된 아이템이 존재하고 아이템을 사용 중이지 않은 경우
        if (Input.GetButton("UseItem") && _curItem != null && !_isRunning)
        {
            _isRunning = true;
            _curItem.Use(this.gameObject);
        }
    }
}
