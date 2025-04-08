using UnityEngine;

public class Ore : Item
{
    public OreType oreType; // Type of ore (e.g., copper, iron, etc.)

    private void OnEnable()
    {
        type = ItemType.Consumable; // Optional: classify ores
        cursorMode = CursorMode.None; // You don’t “use” ores
    }

    public override void Use(GridPosition target)
    {
        Debug.Log("Ore doesn’t do anything on use.");
    }
}
