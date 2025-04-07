using UnityEngine;

[CreateAssetMenu(fileName = "DrillItem", menuName = "Items/DrillItem")]
public class DrillItem : Item
{
    public GameObject drillPrefab; // Prefab for the drill item

    private void OnEnable()
    {
        type = ItemType.Drill; // Set the item type to Drill
        cursorMode = CursorMode.Drill; // Set the cursor mode to Auto
    }

    public override void Use(GridPosition target)
    {
        GridManager grid = FindFirstObjectByType<GridManager>();
        Debug.Log($"DrillItem used at {target}"); // Log the target position
        grid.MineAt(target); // Call the MineAt method on the grid manager
    }
}
