using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventorySlot
{
    public ItemSO itemData;
    public ItemTile script;

    public InventorySlot(ItemTile tile)
    {
        this.itemData = tile.itemSO;
        this.script = tile;
    }
}

[CreateAssetMenu(menuName = "InventorySO",fileName = "InventorySO")]
public class InventorySO : ScriptableObject
{
    public List<InventorySlot> InventorySlots = new List<InventorySlot>();
    public delegate void ItemAddedEvent(InventorySlot slot);
    public event ItemAddedEvent OnItemAdd;
    
    public delegate void ItemUsedEvent(InventorySlot slot);
    public event ItemUsedEvent OnItemUsed;
    
    void OnEnable()
    {
        InventorySlots.Add(null);
        InventorySlots.Add(null);
    }

    void OnDisable()
    {
        InventorySlots.Clear();
    }

    public void AddItemEvent(ItemTile item)
    {
        InventorySlot slot = new InventorySlot(item);
        OnItemAdd?.Invoke(slot);
    }

    public void UseItemEvent(InventorySlot slot)
    {
        OnItemUsed?.Invoke(slot);
    }
}



