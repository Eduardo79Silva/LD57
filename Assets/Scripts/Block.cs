using UnityEngine;

// Represents an individual block that tracks its own structural properties.
public class Block : MonoBehaviour
{
    public int gridX,
        gridY;
    public int supportStrength = 5; // How much load this block can support.
    public float torqueFactor = 0.5f; // Factor for torque calculation.
    public float weight = 1f; // Weight of the block itself.

    [HideInInspector]
    public GridManager gridManager;

    // Initialize the block with its grid coordinates and a reference to the grid manager.
    public void Init(int x, int y, GridManager manager)
    {
        gridX = x;
        gridY = y;
        gridManager = manager;
    }

    // Evaluate the block's stability. This method uses a simplified model:
    // 1. Checks for direct support below.
    // 2. If unsupported, calculates indirect (lateral) support.
    // 3. Also estimates a "torque" effect based on how far the block is laterally offset from support.
    public bool EvaluateStability()
    {
        bool hasDirectSupport = HasDirectSupport();

        if (hasDirectSupport)
            return true;

        float indirectSupport = 0f;
        if (gridManager.IsInBounds(gridX - 1, gridY))
        {
            GameObject left = gridManager.gridArray[gridX - 1, gridY];
            if (left != null && IsBlockSupported(left))
                indirectSupport += 1f;
        }
        // Check right neighbor.
        if (gridManager.IsInBounds(gridX + 1, gridY))
        {
            GameObject right = gridManager.gridArray[gridX + 1, gridY];
            if (right != null && IsBlockSupported(right))
                indirectSupport += 1f;
        }

        // Estimate the weight above this block.
        int weightAbove = CountBlocksAbove();

        // Determine the support needed.
        float requiredSupport = (weightAbove * 0.5f) + weight;

        // Estimate torque based on lateral distance from support.
        float leftDistance = GetSupportDistance(-1);
        float rightDistance = GetSupportDistance(1);
        float torque = Mathf.Max(leftDistance, rightDistance) * torqueFactor * weightAbove;

        // If indirect support is insufficient or torque is too high, the block collapses.
        if (indirectSupport < requiredSupport || torque > supportStrength)
        {
            Collapse();
            return false; // Block is unstable and collapses.
        }
        return true; // Block is stable.
    }

    // Check if there is a block directly below.
    public bool HasDirectSupport()
    {
        if (gridManager.IsInBounds(gridX, gridY - 1))
            return gridManager.gridArray[gridX, gridY - 1] != null;
        return false;
    }

    // Count the number of blocks directly above this one.
    private int CountBlocksAbove()
    {
        int count = 0;
        for (int y = gridY + 1; y < gridManager.GridHeightInCells; y++)
        {
            if (gridManager.gridArray[gridX, y] != null)
                count++;
        }
        return count;
    }

    // Determine if a neighboring block is supported (i.e. has a block below it).
    private bool IsBlockSupported(GameObject blockObj)
    {
        if (blockObj.TryGetComponent<Block>(out var neighbor))
        {
            if (gridManager.IsInBounds(neighbor.gridX, neighbor.gridY - 1))
                return gridManager.gridArray[neighbor.gridX, neighbor.gridY - 1] != null;
        }
        return false;
    }

    // Calculate how far (in cells) you must go in a given horizontal direction (-1 for left, 1 for right)
    // before finding a block with direct support.
    private float GetSupportDistance(int direction)
    {
        float distance = 0f;
        int currentX = gridX;
        while (gridManager.IsInBounds(currentX, gridY))
        {
            if (gridManager.gridArray[currentX, gridY] != null)
            {
                Block b = gridManager.gridArray[currentX, gridY].GetComponent<Block>();
                if (b != null && b.HasDirectSupport())
                    break;
            }
            distance += 1f;
            currentX += direction;
        }
        return distance;
    }

    public void Collapse()
    {
        gridManager.RemoveBlockAt(gridX, gridY);
    }
}
