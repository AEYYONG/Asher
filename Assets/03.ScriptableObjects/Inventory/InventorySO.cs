using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventorySlot
{
    
    public TileSO itemData;
    public Tile script;

    public InventorySlot(Tile tile)
    {
        this.itemData = tile.tileSO;
        this.script = tile;
    }

    public InventorySlot(TileSO tileSO)
    {
        this.itemData = tileSO;
        this.script = null;
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

    public void AddItemEvent(Tile item)
    {
        InventorySlot slot = new InventorySlot(item);
        OnItemAdd?.Invoke(slot);
    }

    public void UseItemEvent(InventorySlot slot)
    {
        OnItemUsed?.Invoke(slot);
    }
    
}



