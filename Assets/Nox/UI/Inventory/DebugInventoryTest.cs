
using UnityEngine;

public class DebugInventoryTest : MonoBehaviour
{
    public InventoryUIManager inventoryUIManager; // Reference to the UI manager
    public Inventory PlayerInventory; // Reference to the player's inventory

    public Item itemToAdd; // Direct reference to the item you want to add, set this in the Inspector

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AddItemToInventory();
        }
    }

    public void AddItemToInventory()
    {
        if (PlayerInventory.AddItem(itemToAdd))
        {
            Debug.Log($"{itemToAdd.Name} was added to the inventory.");
            inventoryUIManager.UpdateInventoryDisplay();
        }
        else
        {
            Debug.Log("No empty slot found for the item.");
        }
    }
}
