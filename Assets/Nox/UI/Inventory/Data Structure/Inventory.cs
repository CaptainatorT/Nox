using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Inventory/New Inventory")]
public class Inventory : ScriptableObject
{
    public int Capacity = 24;
    public List<ItemSlot> Slots = new List<ItemSlot>();
    public InventoryConfig inventoryConfig;

    private void OnEnable()
    {
        if (Slots.Count != Capacity)
        {
            Slots = new List<ItemSlot>();
            for (int i = 0; i < Capacity; i++)
            {
                Slots.Add(new ItemSlot());
            }
        }
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        ItemSlot emptySlot = FindFirstEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.AddItem(item);
            return true;
        }
        else
        {
            return false; // Inventory full
        }
    }

    public bool RemoveItem(Item item, int quantity = 1)
    {
        int removedCount = 0;
        for (int i = 0; i < quantity; i++)
        {
            ItemSlot itemSlot = Slots.Find(slot => slot.ContainedItem != null && slot.ContainedItem.ID == item.ID);
            if (itemSlot != null)
            {
                itemSlot.RemoveItem();
                removedCount++;
            }
        }
        return removedCount == quantity;
    }

    private ItemSlot FindFirstEmptySlot()
    {
        return Slots.Find(slot => slot.IsEmpty);
    }
}
