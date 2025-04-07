using UnityEngine;

public enum ItemType
{
    Drill,
    Magnet,
    Support,
    Consumable,
}

public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public Texture2D cursorIcon;
    public ItemType type;
    public CursorMode cursorMode;

    public virtual void Use(GridPosition target)
    {
        Debug.Log("Using item at " + target);
    }
}
