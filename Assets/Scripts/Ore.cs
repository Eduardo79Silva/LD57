using UnityEngine;

[CreateAssetMenu(fileName = "NewOre", menuName = "Items/Ore")]
public class Ore : Item
{
    public int sellValue;

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
