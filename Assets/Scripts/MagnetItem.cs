using UnityEngine;

[CreateAssetMenu(fileName = "NewMagnetItem", menuName = "Items/Magnet")]
public class MagnetItem : Item
{
    public float pullStrength = 5f;
    public float pullRadius = 3f;

    private void OnEnable()
    {
        type = ItemType.Magnet;
        cursorMode = CursorMode.Magnet;
    }

    public override void Use(GridPosition target)
    {
        // When clicked, apply a stronger magnetic pull at the target location
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager != null)
        {
            // Apply a stronger pulling force when clicked
            Vector2 center = target.ToWorld();
            float radius = pullRadius;

            Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<OrePickup>(out var orePickup))
                {
                    Vector2 pullDirection = (
                        center - (Vector2)orePickup.transform.position
                    ).normalized;
                    orePickup.ApplyMagnetForce(pullDirection * pullStrength);
                }
            }
        }

        Debug.Log($"Used magnet at {target} with pull strength {pullStrength}");
    }
}
