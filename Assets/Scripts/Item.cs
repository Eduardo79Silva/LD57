using UnityEngine;

public enum ItemType
{
    Drill,
    Magnet,
    Support,
    Consumable,
}

public class Item : MonoBehaviour
{
    public string itemName;
    public string description;
    public int maxStackSize = 1;
    public int sellValue = 0;
    public int amount = 1;
    public Sprite icon;
    public Texture2D cursorIcon;
    public ItemType type;
    public CursorMode cursorMode;

    public virtual void Use(GridPosition target)
    {
        Debug.Log("Using item at " + target);
    }
}
