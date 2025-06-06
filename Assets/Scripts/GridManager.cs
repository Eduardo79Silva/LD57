using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    public float cellSize;
    public float downwardsSizeOfGrid = 10f;
    public ProceduralBlockFactory blockFactory; // Reference to our block generator

    [HideInInspector]
    public GameObject[,] gridArray;
    public int GridWidthInCells { get; private set; }
    public int GridHeightInCells { get; private set; }
    private int totalBlocksInGrid;
    private int totalOreBlocksInGrid;

    public Canvas gameOverCanvas; // Reference to the game over canvas
    public TextMeshProUGUI scoreText; // Reference to the score text

    public Sprite grassImage; // Reference to the grass image for the game over screen

    [HideInInspector]
    public Vector3 gridOrigin;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Calculate screen dimensions using the orthographic camera.
        Camera cam = Camera.main;
        blockFactory.Initialization(this);

        float screenHeight = cam.orthographicSize * 2f;
        float screenWidth = screenHeight * cam.aspect;

        float gridWorldHeight = screenHeight * downwardsSizeOfGrid;
        float gridWorldWidth = screenWidth;

        // Calculate cell size based on the screen size and desired grid dimensions.
        cellSize = Mathf.Min(gridWorldWidth / 21f, gridWorldHeight / 21f); // 10 cells in each direction

        GridWidthInCells = Mathf.CeilToInt(gridWorldWidth / cellSize);
        GridHeightInCells = Mathf.CeilToInt(gridWorldHeight / cellSize);
        gridArray = new GameObject[GridWidthInCells, GridHeightInCells];

        // Position the grid so that its top edge aligns with the top of the screen.
        Vector3 topCenter = new(
            cam.transform.position.x,
            cam.transform.position.y + screenHeight / 2f,
            0
        );
        gridOrigin = new Vector3(
            topCenter.x - gridWorldWidth / 2f,
            topCenter.y - gridWorldHeight,
            0
        );
        transform.position = gridOrigin;

        // Generate terrain (with ores) procedurally.
        GenerateTerrain();

        // (Optional) Draw grid lines for visualization.
        DrawGrid(gridWorldWidth, gridWorldHeight);
    }

    void Update()
    {
        // Check for game over condition.
        if (isGameOver())
        {
            int finalScore = CalculateFinalScore();
            Debug.Log($"Game Over! Final Score: {finalScore}");
            // Handle game over logic here (e.g., show UI, reset game, etc.)
            gameOverCanvas.gameObject.SetActive(true);
            scoreText.text = $"{finalScore}";
        }
    }

    bool isGameOver()
    {
        // Check if the game is over (e.g., all blocks are removed).
        for (int x = 0; x < GridWidthInCells; x++)
        {
            for (int y = 0; y < GridHeightInCells; y++)
            {
                if (gridArray[x, y] == null)
                {
                    continue;
                }
                if (gridArray[x, y].GetComponent<Block>().oreType != OreType.Dirt)
                {
                    // If any block has an ore type, the game is not over.
                    return false;
                }
            }
        }
        Debug.Log("Game Over! All ores removed.");
        return true;
    }

    int CalculateFinalScore()
    {
        int score = 0;
        int keptDirtBlocks = 0;
        for (int x = 0; x < GridWidthInCells; x++)
        {
            for (int y = 0; y < GridHeightInCells; y++)
            {
                if (gridArray[x, y] != null)
                {
                    Block block = gridArray[x, y].GetComponent<Block>();
                    if (block != null && block.oreType == OreType.Dirt)
                    {
                        keptDirtBlocks++;
                    }
                }
            }
        }
        int oreScore =
            InventoryManager.Instance.ironCount * 5
            + InventoryManager.Instance.goldCount * 10
            + InventoryManager.Instance.diamondCount * 20;
        int oreCount =
            InventoryManager.Instance.ironCount
            + InventoryManager.Instance.goldCount
            + InventoryManager.Instance.diamondCount;
        // Calculate score based on the number of ores collected compared to initial ores and kept dirt blocks plus the ore score.
        score = (totalOreBlocksInGrid - oreCount) * 10 + keptDirtBlocks * 2 + oreScore;

        score = Mathf.Max(0, score); // Ensure score is non-negative

        return score;
    }

    void GenerateTerrain()
    {
        float noiseScale = 0.1f; // adjust for desired roughness
        float noiseOffset = Random.Range(0f, 1000f);

        for (int y = 0; y < GridHeightInCells; y++)
        {
            // Compute a ground height for this column using Perlin noise.

            // Fill in all cells below (and including) groundHeight.
            for (int x = 0; x < GridWidthInCells; x++)
            {
                float noiseValue = Mathf.PerlinNoise((x + noiseOffset) * noiseScale, 0f);
                int groundHeight = Mathf.FloorToInt(
                    noiseValue * (GridHeightInCells / 2) + GridHeightInCells * 0.6f
                );
                if (y <= groundHeight)
                {
                    Vector3 spawnPos = new(
                        gridOrigin.x + x * cellSize + cellSize / 2f,
                        gridOrigin.y + y * cellSize + cellSize / 2f,
                        0
                    );
                    // Compute depth ratio relative to the column's top block.
                    // When y == groundHeight, depthRatio = 0 (surface)
                    // When y == 0, depthRatio = 1 (deepest)
                    float depthRatio = (float)(groundHeight - y) / groundHeight;

                    // Pass depthRatio into the block factory.
                    GameObject block = blockFactory.GenerateBlock(x, y, depthRatio, spawnPos);
                    gridArray[x, y] = block;
                    if (block == null)
                    {
                        Debug.LogError($"Block generation failed at ({x}, {y})");
                        continue;
                    }

                    if (block.TryGetComponent<Block>(out var blockComp))
                    {
                        blockComp.Init(x, y, this);
                        totalBlocksInGrid++;
                        if (blockComp.oreType != OreType.Dirt)
                        {
                            totalOreBlocksInGrid++;
                        }
                    }

                    // if it is a top block, change sprite to be a top block
                    if (y == groundHeight)
                    {
                        if (
                            block.TryGetComponent<SpriteRenderer>(out var spriteRenderer)
                            && grassImage != null
                        )
                        {
                            // Set the sprite to the grass image
                            spriteRenderer.sprite = grassImage;
                        }
                        {
                            spriteRenderer.sprite = grassImage;
                        }
                    }
                }
            }
        }
    }

    void DrawGrid(float gridWorldWidth, float gridWorldHeight)
    {
        // Draw vertical grid lines.
        for (int x = 0; x <= GridWidthInCells; x++)
        {
            Vector3 start = new(gridOrigin.x + x * cellSize, gridOrigin.y, 0);
            Vector3 end = new(gridOrigin.x + x * cellSize, gridOrigin.y + gridWorldHeight, 0);
            Debug.DrawLine(start, end, Color.gray, 100f);
        }
        // Draw horizontal grid lines.
        for (int y = 0; y <= GridHeightInCells; y++)
        {
            Vector3 start = new(gridOrigin.x, gridOrigin.y + y * cellSize, 0);
            Vector3 end = new(gridOrigin.x + gridWorldWidth, gridOrigin.y + y * cellSize, 0);
            Debug.DrawLine(start, end, Color.gray, 100f);
        }
    }

    public Block GetBlockAt(int x, int y)
    {
        if (IsInBounds(x, y) && gridArray[x, y] != null)
        {
            return gridArray[x, y].GetComponent<Block>();
        }
        return null;
    }

    // Public method to “dig” (remove) a block at a given grid coordinate.
    public void RemoveBlockAt(int x, int y)
    {
        if (IsInBounds(x, y) && gridArray[x, y] != null)
        {
            // Remove the block.
            Destroy(gridArray[x, y]);
            gridArray[x, y] = null;

            // After removal, update tension for neighboring blocks.
            UpdateTension(x, y);
        }
    }

    // Check if the coordinates are within grid bounds.
    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < GridWidthInCells && y < GridHeightInCells;
    }

    // Check and update nearby blocks for tension/stability changes.
    public void UpdateTension(int removedX, int removedY)
    {
        int radius = 3; // adjust how far out you check for support changes.
        Block[] collapsedBlocks = new Block[gridArray.Length];
        for (
            int x = Mathf.Max(0, removedX - radius);
            x < Mathf.Min(GridWidthInCells, removedX + radius);
            x++
        )
        {
            for (
                int y = Mathf.Max(0, removedY - radius);
                y < Mathf.Min(GridHeightInCells, removedY + radius);
                y++
            )
            {
                if (gridArray[x, y] != null)
                {
                    if (gridArray[x, y].TryGetComponent<Block>(out var b))
                    {
                        if (!b.EvaluateStability())
                        {
                            // If the block is unstable, add it to the list of blocks to collapse.
                            collapsedBlocks[x * GridHeightInCells + y] = b;
                        }
                    }
                }
            }
        }

        if (collapsedBlocks.Length > 0)
        {
            // Collapse all blocks that are no longer stable.
            foreach (var block in collapsedBlocks)
            {
                if (block != null)
                {
                    Debug.Log("Block at (" + block.gridX + "," + block.gridY + ") collapsed!");
                    UpdateTension(block.gridX, block.gridY);
                }
            }
        }
    }

    public void PlaceSupport(GridPosition pos, Item supportItem)
    {
        if (!IsInBounds(pos.x, pos.y))
            return;

        if (gridArray[pos.x, pos.y] == null)
        {
            Vector3 spawnPos = new(
                gridOrigin.x + pos.x * cellSize + cellSize / 2f,
                gridOrigin.y + pos.y * cellSize + cellSize / 2f,
                0
            );

            GameObject supportGO = Instantiate(
                ((SupportItem)supportItem).supportPrefab,
                spawnPos,
                Quaternion.identity
            );

            gridArray[pos.x, pos.y] = supportGO;

            InventoryManager.Instance.RemoveItem(supportItem);
            Debug.Log($"Placed support structure at {pos}");
        }
        else
        {
            Debug.Log("Cannot place support — space is occupied.");
        }
    }

    public bool CanPlaceSupport(GridPosition pos)
    {
        if (!IsInBounds(pos.x, pos.y))
            return false;

        // Check if the cell is empty (no GameObject placed there)
        return gridArray[pos.x, pos.y] == null;
    }

    public void PullOre(GridPosition pos)
    {
        Vector2 center = pos.ToWorld();
        float radius = cellSize * 1.5f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<OrePickup>(out var orePickup))
            {
                Vector2 pullDirection = (center - (Vector2)orePickup.transform.position).normalized;
                orePickup.ApplyMagnetForce(pullDirection);
            }
        }
    }

    public void MineAt(GridPosition pos)
    {
        Debug.Log($"Mining at {pos}");
        if (!IsInBounds(pos.x, pos.y))
            return;

        Block targetBlock = GetBlockAt(pos.x, pos.y);
        Debug.Log($"Target block: {targetBlock}");
        if (targetBlock != null && targetBlock.isMineable)
        {
            RemoveBlockAt(pos.x, pos.y);
        }

        InventoryManager.Instance.UpdateItemCountText(targetBlock);
    }
}

public struct GridPosition
{
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static GridPosition FromWorld(Vector2 worldPos)
    {
        if (GridManager.Instance == null)
        {
            Debug.LogError(
                "GridManager.Instance is null! Make sure GridManager is active in the scene."
            );
            return default;
        }

        int x = Mathf.FloorToInt(
            (worldPos.x - GridManager.Instance.gridOrigin.x) / GridManager.Instance.cellSize
        );
        int y = Mathf.FloorToInt(
            (worldPos.y - GridManager.Instance.gridOrigin.y) / GridManager.Instance.cellSize
        );
        return new GridPosition(x, y);
    }

    public static GridPosition GetMousePositionInGrid()
    {
        // Get the mouse position in world coordinates and convert it to grid coordinates.
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return FromWorld(mouseWorldPos);
    }

    public Vector2 ToWorld()
    {
        return new Vector2(x + 0.5f, y + 0.5f); // Center of tile
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.y + b.y);
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
}
