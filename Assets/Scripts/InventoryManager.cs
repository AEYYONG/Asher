using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class InventoryManager : MonoBehaviour
{
    //인벤토리 내 두개의 슬롯
    public Image firstSlot;
    public Image secondSlot;
    
    //인벤토리 내 슬롯 채워져 있는지 여부
    public bool isFirstSlotFull = false;

    public InventorySO inventory;

    public TileSO emptyItemSO;

    private InventorySlot emptySlot;

    private bool _isSwap = false;
    private Animator _animator;
    [SerializeField] private InventorySlot _curSlot;

    void Awake()
    {
        emptySlot = new InventorySlot(emptyItemSO);
        _animator = GetComponent<Animator>();
    }
    void Start()
    {
        inventory.InventorySlots[0] = emptySlot;
        inventory.InventorySlots[1] = emptySlot;
        _curSlot = inventory.InventorySlots[0];
    }
    
    
    void Update()
    {
        //아이템 Swap 버튼을 클릭한 경우
        if (Input.GetButtonUp("SwapItem"))
        {
            if (_isSwap)
            {
                _animator.SetTrigger("ReSwap");
                _isSwap = false;
                _curSlot = inventory.InventorySlots[0];
            }
            else
            {
                _animator.SetTrigger("Swap");
                _isSwap = true;
                _curSlot = inventory.InventorySlots[1];
            }
            Debug.Log("아이템 Swap");
            SwapSlot();
        }
        
        //아이템 사용 버튼을 클릭하였고 현재 선택된 아이템이 존재하고 아이템을 사용 중이지 않은 경우
        if (Input.GetButtonUp("UseItem") && isFirstSlotFull)
        {
            UseItem();
        }
    }

    
    void OnEnable()
    {
        inventory.OnItemAdd += AddItem;
    }
    
    void OnDisable()
    {
        inventory.OnItemAdd -= AddItem;
    }

    //인벤토리 슬롯1과 슬롯2의 위치 변경
    void SwapSlot()
    {
        //슬롯 값 교환
        /*
        (inventory.InventorySlots[0], inventory.InventorySlots[1]) =
            (inventory.InventorySlots[1], inventory.InventorySlots[0]);
        */
        if (_curSlot.itemData == emptyItemSO)
        {
            //Debug.Log("swap debug");
            isFirstSlotFull = false;
        }
        else
        {
            isFirstSlotFull = true;
        }
        //UI에 슬롯 정보 업데이터
        UpdateSlotUI();
    }

    public void AddItem(InventorySlot item)
    {
        if (isFirstSlotFull)
        {
            if (_isSwap)
            {
                inventory.InventorySlots[0] = item;
            }
            else
            {
                inventory.InventorySlots[1] = item; 
            }
        }
        else
        {
            if (_isSwap)
            {
                inventory.InventorySlots[1] = item;
            }
            else
            {
                inventory.InventorySlots[0] = item; 
            }
            isFirstSlotFull = true;
        }
        UpdateSlotUI();
    }
    
    //아이템 사용 시, 슬롯 비우기
    public void ClearItem()
    {
        _curSlot.itemData = emptyItemSO;
        _curSlot.script = null;
        isFirstSlotFull = false;
    }

    public void UpdateSlotUI()
    {
        InventorySlot slot1 = inventory.InventorySlots[0];
        InventorySlot slot2 = inventory.InventorySlots[1];
        //Debug.Log("slot1" + slot1);
        //Debug.Log("slot2"+slot2);
        if (slot1.itemData != emptyItemSO)
        {
            firstSlot.sprite = slot1.itemData.itemImg;
        }
        else
        {
            firstSlot.sprite = emptyItemSO.itemImg;
        }
        
        if (slot2.itemData != emptyItemSO)
        {
            secondSlot.sprite = slot2.itemData.itemImg;
        }
        else
        {
            secondSlot.sprite = emptyItemSO.itemImg;
        }

        if (_isSwap)
        {
            _curSlot = inventory.InventorySlots[1];
        }
        else{
            _curSlot = inventory.InventorySlots[0];
        }
    }

    public void UseItem()
    {
        InventorySlot item = _curSlot;
        if (item.itemData != emptyItemSO)
        {
            inventory.UseItemEvent(item);
            ClearItem();
            UpdateSlotUI();
        }
    }

    public int GetItemCnt()
    {
        int cnt = 0;
        InventorySlot slot1 = inventory.InventorySlots[0];
        InventorySlot slot2 = inventory.InventorySlots[1];
        //Debug.Log("slot1" + slot1);
        //Debug.Log("slot2"+slot2);
        if (slot1.itemData != emptyItemSO)
        {
            cnt++;
        }
        if (slot2.itemData != emptyItemSO)
        {
            cnt++;
        }

        return cnt;
    }

    public void SelectFeverTimeItem(Tile item)
    {
        _curSlot.itemData = item.tileSO;
        _curSlot.script = item;
        UpdateSlotUI();
    }
}
