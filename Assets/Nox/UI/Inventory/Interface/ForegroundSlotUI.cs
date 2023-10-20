using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ForegroundSlotUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public InventorySlotUI parentInventorySlot;
    public bool isDragging = false;
    private Vector2 originalPosition;
    private RectTransform rectTransform;
    private int originalSiblingIndex;

    private GameObject draggedItemRepresentation; // The visual representation of the item being dragged

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (parentInventorySlot.currentItem == null) return;

        Debug.Log($"[Foreground OnBeginDrag] Starting to drag item: {parentInventorySlot.currentItem.Name}");

        // Hide original item representation
        parentInventorySlot.ForegroundImage.enabled = false;

        CreateDraggedItemRepresentation();
        isDragging = true;
        transform.SetAsLastSibling();
        originalPosition = rectTransform.anchoredPosition;
        originalSiblingIndex = transform.GetSiblingIndex();

        parentInventorySlot.transform.SetAsLastSibling();
        GetComponent<Image>().raycastTarget = false;

        Debug.Log("Begin Drag");
    }

    private void CreateDraggedItemRepresentation()
    {
        if (draggedItemRepresentation != null)
        {
            Destroy(draggedItemRepresentation);
        }

        Canvas mainCanvas = parentInventorySlot.uiManager.InventoryPanel.GetComponentInParent<Canvas>();
        draggedItemRepresentation = new GameObject("DraggedItem");
        draggedItemRepresentation.transform.SetParent(mainCanvas.transform);
        draggedItemRepresentation.transform.SetAsLastSibling();

        RectTransform draggedRectTransform = draggedItemRepresentation.AddComponent<RectTransform>();
        draggedRectTransform.sizeDelta = rectTransform.sizeDelta;
        Image draggedImage = draggedItemRepresentation.AddComponent<Image>();
        draggedImage.sprite = parentInventorySlot.currentItem.Icon;
        draggedImage.color = parentInventorySlot.ForegroundImage.color;
        draggedImage.raycastTarget = false;
    }

    private void Update()
    {
        if (isDragging && draggedItemRepresentation)
        {
            // Update the position of the dragged representation to follow the mouse cursor
            Canvas canvas = parentInventorySlot.uiManager.InventoryPanel.GetComponentInParent<Canvas>();
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out position);
            draggedItemRepresentation.GetComponent<RectTransform>().position = canvas.transform.TransformPoint(position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"[Foreground OnEndDrag] Ended dragging item: {parentInventorySlot.currentItem.Name}");
        isDragging = false;
        GetComponent<Image>().raycastTarget = true;

        // Show original item representation
        parentInventorySlot.ForegroundImage.enabled = true;

        InventorySlotUI targetSlot = eventData.pointerCurrentRaycast.gameObject?.GetComponent<InventorySlotUI>();

        if (targetSlot && targetSlot.IsEmpty())
        {
            targetSlot.SetItem(parentInventorySlot.currentItem);
            parentInventorySlot.ClearSlot();
        }
        else
        {
            rectTransform.anchoredPosition = originalPosition;
        }

        transform.SetSiblingIndex(originalSiblingIndex);
        parentInventorySlot.transform.SetAsLastSibling();

        if (draggedItemRepresentation != null)
        {
            Destroy(draggedItemRepresentation);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Item Dropped");
    }
}
