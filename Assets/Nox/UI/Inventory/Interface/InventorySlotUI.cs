using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public Item currentItem { get; private set; }
    public Image BackgroundImage;
    public Image ForegroundImage;
    public InventoryUIManager uiManager;
    public TooltipScript tooltipScript; // Reference to the TooltipScript for displaying tooltips

    private Vector2 startPosition;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetItem(Item item)
    {
        currentItem = item;

        if (item != null)
        {
            ForegroundImage.sprite = item.Icon;
            ForegroundImage.gameObject.SetActive(true);
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        ForegroundImage.sprite = null;
        ForegroundImage.gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        tooltipScript.HideTooltip(); // Hide tooltip when dragging

        startPosition = rectTransform.position;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventorySlotUI dropSlot = eventData.pointerCurrentRaycast.gameObject?.GetComponent<InventorySlotUI>();

        if (dropSlot && dropSlot.IsEmpty())
        {
            dropSlot.SetItem(currentItem);
            ClearSlot();
        }
        else
        {
            rectTransform.position = startPosition;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI droppedSlot = eventData.pointerDrag.GetComponent<InventorySlotUI>();

        if (droppedSlot && IsEmpty())
        {
            SetItem(droppedSlot.currentItem);
            droppedSlot.ClearSlot();
        }
    }

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    // Tooltip methods

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem is Weapon) // If the item is a Weapon type
        {
            Weapon weapon = currentItem as Weapon;
            string tooltipContent = GenerateWeaponTooltip(weapon);

            // Set the position of the tooltip with an offset
            Vector3 offset = new Vector3(10, -10, 0);
            Vector2 tooltipSize = TooltipScript.Instance.tooltipPanel.GetComponent<RectTransform>().sizeDelta;
            TooltipScript.Instance.tooltipPanel.transform.position = Input.mousePosition + new Vector3(offset.x, offset.y - tooltipSize.y, offset.z);

            TooltipScript.Instance.ShowTooltip(tooltipContent); // Use the Singleton instance
        }
        // Add more conditions for other item types as needed
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipScript.Instance.HideTooltip();
    }


    private string GenerateWeaponTooltip(Weapon weapon)
    {
        string tooltipContent = "";
        tooltipContent += "Name: " + weapon.Name + "\n";
        tooltipContent += "Fire Rate: " + weapon.fireRate + "\n";
        tooltipContent += "Reload Time: " + weapon.reloadTime + "s\n";
        tooltipContent += "Damage: " + weapon.damage + "\n";
        // ... Add more relevant stats for the weapon...

        return tooltipContent;
    }
}
