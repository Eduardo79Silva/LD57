using UnityEngine;

public class GridManager : MonoBehaviour
{
    public float cellSize = 1f;
    public ProceduralBlockFactory blockFactory; // Reference to our block generator

    [HideInInspector]
    public GameObject[,] gridArray;
    public int GridWidthInCells { get; private set; }
    public int GridHeightInCells { get; private set; }

    [HideInInspector]
    public Vector3 gridOrigin;

    void Start()
    {
        // Calculate screen dimensions using the orthographic camera.
        Camera cam = Camera.main;
        blockFactory.Initialization(this);

        float screenHeight = cam.orthographicSize * 2f;
        float screenWidth = screenHeight * cam.aspect;

        // Grid: as wide as the screen and twice as tall (extending downward).
        float gridWorldHeight = screenHeight * 2f;
        float gridWorldWidth = screenWidth;

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

    void GenerateTerrain()
    {
        float noiseScale = 0.1f; // adjust for desired roughness
        float noiseOffset = Random.Range(0f, 1000f);

        for (int x = 0; x < GridWidthInCells; x++)
        {
            // Compute a ground height for this column using Perlin noise.
            float noiseValue = Mathf.PerlinNoise((x + noiseOffset) * noiseScale, 0f);
            int groundHeight =
                Mathf.FloorToInt(noiseValue * (GridHeightInCells / 2)) + GridHeightInCells / 4;

            // Fill in all cells below (and including) groundHeight.
            for (int y = 0; y < GridHeightInCells; y++)
            {
                if (y <= groundHeight)
                {
                    Vector3 spawnPos = new Vector3(
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

                    if (block.TryGetComponent<Block>(out var blockComp))
                    {
                        blockComp.Init(x, y, this);
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

    // For testing: dig a block where the player clicks.
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.FloorToInt((mouseWorldPos.x - gridOrigin.x) / cellSize);
            int y = Mathf.FloorToInt((mouseWorldPos.y - gridOrigin.y) / cellSize);
            RemoveBlockAt(x, y);
        }
    }
}
