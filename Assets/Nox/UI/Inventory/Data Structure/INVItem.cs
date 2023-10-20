using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item")]
public class Item : ScriptableObject
{
    public int ID;
    public string Name;
    public string Description;
    public Sprite Icon;
    public ItemType Type;
    public bool IsStackable;
    // Add more properties as needed (e.g., item stats if it's a weapon, etc.)
}
