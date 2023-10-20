using UnityEngine;

public class ItemSlot
{
    public Item ContainedItem { get; private set; }
    public int Quantity { get; private set; }

    public bool IsEmpty => ContainedItem == null;

    public void AddItem(Item item, int quantity = 1)
    {
        ContainedItem = item;
        Quantity += quantity;
    }

    public void RemoveItem(int quantity = 1)
    {
        Quantity -= quantity;
        if (Quantity <= 0)
        {
            Clear();
        }
    }

    public void Clear()
    {
        ContainedItem = null;
        Quantity = 0;
    }

    // The additional method for direct setting. Useful for debugging.
    public void SetItemDirectly(Item item)
    {
        ContainedItem = item;
        if (item != null)
        {
            Quantity = 1;
        }
        else
        {
            Quantity = 0;
        }
    }
}
