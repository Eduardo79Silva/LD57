using System.Collections.Generic;
using UnityEngine;

public enum OreType
{
    Dirt,
    Iron,
    Gold,
    Diamond,
}

// Represents an individual block that tracks its own structural properties.
public class Block : MonoBehaviour
{
    public OreType oreType;
    public bool isMineable = true;
    public bool isBrittle = false; // If true, the block is more likely to break under tension.
    public float maxStressCapacity = 10.0f; // Maximum stress capacity of the block.
    public float currentStress = 0.0f; // Current stress on the block.
    public float lateralConnectionStrength = 0.7f; // Connection strength to blocks on the same level.
    public float diagonalConnectionStrength = 0.3f; // Connection strength to diagonal blocks.
    public float dampingFactor = 0.0f; // How much this material absorbs/reduces force transfer.
    public int gridX,
        gridY;
    public int supportStrength = 5; // How much tension the block can handle.
    public float torqueFactor = 0.3f; // Additional torque from lateral gaps.
    public float weight = 1f; // Weight of the block itself.

    // Multipliers for support contributions.
    private const float directSupportValue = 1f;
    private const float diagonalSupportValue = 0.7f;

    // Multiplier to convert support into load reduction.
    private const float supportMultiplier = 3.0f;

    [HideInInspector]
    public GridManager gridManager;

    // Initialize the block with its grid coordinates and a reference to the grid manager.
    public void Init(int x, int y, GridManager manager)
    {
        gridX = x;
        gridY = y;
        gridManager = manager;
    }

    /// <summary>
    /// Evaluate the block's stability using a two-part approach:
    /// (1) Check downward connectivity (only allow downward moves).
    /// (2) Compute vertical load versus lateral support.
    /// Only blocks that lose vertical connectivity will collapse,
    /// and then only the blocks above (dependent on that support) will fall.
    /// </summary>
    public bool EvaluateStability()
    {
        // If the block has direct vertical support, it's stable.
        if (HasDirectSupport())
        {
            SetColor(Color.green);
            return true;
        }

        // Check if this block (or its connected structure) has a downward path to the ground.
        if (!IsConnectedDownwards())
        {
            CollapseAndCollapseAbove();
            return false;
        }

        // Calculate vertical load: weight of itself plus blocks above.
        int blocksAbove = CountBlocksAbove();
        float verticalLoad = (blocksAbove + 1) * weight;

        // Compute lateral support from immediate left/right neighbors that are downward-connected.
        float lateralSupport = 0f;
        if (gridManager.IsInBounds(gridX - 1, gridY))
        {
            Block left = gridManager.GetBlockAt(gridX - 1, gridY);
            if (left != null && left.IsConnectedDownwards())
                lateralSupport += directSupportValue;
        }
        if (gridManager.IsInBounds(gridX + 1, gridY))
        {
            Block right = gridManager.GetBlockAt(gridX + 1, gridY);
            if (right != null && right.IsConnectedDownwards())
                lateralSupport += directSupportValue;
        }

        // Optionally add a torque term if there is a gap horizontally.
        float lateralOffset = Mathf.Max(GetSupportDistance(-1), GetSupportDistance(1));
        float lateralTorque = lateralOffset * torqueFactor;

        // Compute effective tension.
        float tension = verticalLoad - (lateralSupport * supportMultiplier) + lateralTorque;

        // If tension exceeds the support strength, then the block (and dependent blocks above) collapse.
        if (tension > supportStrength)
        {
            CollapseAndCollapseAbove();
            return false;
        }
        else
        {
            // Change color based on stability (more green means stable).
            float stability = Mathf.Clamp01(1 - (tension / supportStrength));
            SetColor(Color.Lerp(Color.red, Color.green, stability));
            return true;
        }
    }

    /// <summary>
    /// Checks if there is a block directly below.
    /// </summary>
    public bool HasDirectSupport()
    {
        if (gridManager.IsInBounds(gridX, gridY - 1))
            return gridManager.gridArray[gridX, gridY - 1] != null;
        return false;
    }

    /// <summary>
    /// Checks for diagonal support in a given direction (-1 for bottom-left, 1 for bottom-right).
    /// </summary>
    private bool HasDiagonalSupport(int direction)
    {
        int newX = gridX + direction;
        int newY = gridY - 1;
        if (gridManager.IsInBounds(newX, newY))
            return gridManager.gridArray[newX, newY] != null;
        return false;
    }

    /// <summary>
    /// Count the number of blocks directly above this block.
    /// </summary>
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

    /// <summary>
    /// Calculate horizontal distance until a block with direct support is found.
    /// </summary>
    private float GetSupportDistance(int direction)
    {
        float distance = 0f;
        int currentX = gridX;
        while (gridManager.IsInBounds(currentX, gridY))
        {
            GameObject currentBlock = gridManager.gridArray[currentX, gridY];
            if (currentBlock != null)
            {
                Block b = currentBlock.GetComponent<Block>();
                if (b != null && b.HasDirectSupport())
                    break;
            }
            distance += 1f;
            currentX += direction;
        }
        return distance;
    }

    /// <summary>
    /// Checks if this block is connected downward (only moves with dy <= 0 are allowed)
    /// to a block at the bottom (gridY == 0) or one that has direct support.
    /// </summary>
    public bool IsConnectedDownwards()
    {
        Queue<Block> queue = new();
        HashSet<Block> visited = new();
        queue.Enqueue(this);
        visited.Add(this);

        // Only allow moves that do not increase y (stay same or move downward).
        Vector2Int[] directions = new Vector2Int[]
        {
            new(0, -1),
            new(-1, -1),
            new(1, -1),
        };

        while (queue.Count > 0)
        {
            Block current = queue.Dequeue();
            // If at the bottom or directly supported, then the chain is anchored.
            if (current.gridY == 0 || current.HasDirectSupport())
                return true;

            foreach (var offset in directions)
            {
                int nx = current.gridX + offset.x;
                int ny = current.gridY + offset.y;
                if (gridManager.IsInBounds(nx, ny) && gridManager.gridArray[nx, ny] != null)
                {
                    Block neighbor = gridManager.gridArray[nx, ny].GetComponent<Block>();
                    if (neighbor != null && !visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Collapse this block and trigger collapse for blocks above that depend on it.
    /// </summary>
    public void CollapseAndCollapseAbove()
    {
        // Collapse this block.
        Collapse();

        Block b = gridManager.gridArray[gridX, gridY + 1].GetComponent<Block>();
        if (b != null)
            b.EvaluateStability();
    }

    /// <summary>
    /// Collapse this block (simulate falling or breaking off).
    /// </summary>
    public void Collapse()
    {
        gridManager.RemoveBlockAt(gridX, gridY);
        // Optionally add visual or sound effects here.
    }

    /// <summary>
    /// Helper to set the block's color.
    /// </summary>
    private void SetColor(Color color)
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = color;
    }
}
