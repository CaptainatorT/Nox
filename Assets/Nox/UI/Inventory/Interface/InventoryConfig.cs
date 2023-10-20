using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Inventory Config")]
public class InventoryConfig : ScriptableObject
{
    public int slotsPerRow;
    public Vector2 slotSpacing;
}
