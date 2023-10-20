using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour
{
    public Inventory PlayerInventory;
    public GameObject InventorySlotPrefab;
    public Transform InventoryPanel;
    public InventoryConfig inventoryConfig;

    public float offsetX = 0f;
    public float offsetY = 0f;

    public List<InventorySlotUI> inventorySlotUIs = new List<InventorySlotUI>();
    private bool isInventoryOpen = false;

    private void Start()
    {
        Vector2 slotDimensions = InventorySlotPrefab.GetComponent<RectTransform>().sizeDelta;

        float totalWidth = (inventoryConfig.slotsPerRow * slotDimensions.x) + ((inventoryConfig.slotsPerRow - 1) * inventoryConfig.slotSpacing.x);
        float totalRows = Mathf.CeilToInt(PlayerInventory.Capacity / (float)inventoryConfig.slotsPerRow);
        float totalHeight = (totalRows * slotDimensions.y) + ((totalRows - 1) * inventoryConfig.slotSpacing.y);

        Vector2 startPosition = new Vector2((-totalWidth / 2) + offsetX, (totalHeight / 2) - offsetY);
        for (int i = 0; i < PlayerInventory.Capacity; i++)
        {
            GameObject slot = Instantiate(InventorySlotPrefab, InventoryPanel);
            InventorySlotUI slotUI = slot.GetComponent<InventorySlotUI>();
            slotUI.uiManager = this;
            inventorySlotUIs.Add(slotUI);

            int row = i / inventoryConfig.slotsPerRow;
            int col = i % inventoryConfig.slotsPerRow;
            Vector2 position = startPosition + new Vector2(col * (slotDimensions.x + inventoryConfig.slotSpacing.x), -row * (slotDimensions.y + inventoryConfig.slotSpacing.y));
            slot.GetComponent<RectTransform>().anchoredPosition = position;
        }
        UpdateInventoryDisplay();
        InventoryPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        InventoryPanel.gameObject.SetActive(isInventoryOpen);
    }

    public void UpdateInventoryDisplay()
    {
        int slotCount = Mathf.Min(PlayerInventory.Slots.Count, inventorySlotUIs.Count);
        for (int i = 0; i < slotCount; i++)
        {
            if (PlayerInventory.Slots[i].ContainedItem != null)
            {
                Item currentItem = PlayerInventory.Slots[i].ContainedItem;
                inventorySlotUIs[i].SetItem(currentItem);
            }
            else
            {
                inventorySlotUIs[i].ClearSlot();
            }
        }
    }
}
