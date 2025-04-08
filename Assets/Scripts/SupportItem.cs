using UnityEngine;

public class SupportItem : Item
{
    public GameObject supportPrefab;

    public override void Use(GridPosition target)
    {
        GridManager grid = FindFirstObjectByType<GridManager>();
        grid.PlaceSupport(target, this);
    }
}
