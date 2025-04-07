using UnityEngine;

public enum CursorMode
{
    None,
    Drill,
    Magnet,
    Support,
}

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    public CursorMode currentMode = CursorMode.None;
    public Item currentItem;
    private GridManager gridManager;
    public GameObject supportPreviewPrefab;
    private SupportPreview currentPreview;

    // Add parameters for magnet functionality
    public float magnetRadius = 1.5f;
    public float magnetUpdateInterval = 0.1f;
    private float magnetTimer = 0f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void Initialization(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridPosition pos = GridPosition.FromWorld(mouseWorldPos);

        if (currentItem == null)
            return;

        // Handle magnet functionality
        if (currentMode == CursorMode.Magnet)
        {
            // Use a timer to control how often we pull ores (for performance)
            magnetTimer += Time.deltaTime;
            if (magnetTimer >= magnetUpdateInterval)
            {
                magnetTimer = 0f;
                gridManager.PullOre(pos);
            }

            // Also handle click functionality for magnet items
            if (Input.GetMouseButtonDown(0))
            {
                currentItem.Use(pos);
            }
        }
        // Special logic for SupportItem (with preview)
        else if (currentItem is SupportItem supportItem)
        {
            if (currentPreview == null)
            {
                GameObject previewGO = Instantiate(supportPreviewPrefab);
                currentPreview = previewGO.GetComponent<SupportPreview>();
            }

            currentPreview.SetPosition(pos);
            bool isValid = gridManager.CanPlaceSupport(pos);
            currentPreview.SetValid(isValid);

            if (Input.GetMouseButtonDown(0) && isValid)
            {
                currentItem.Use(pos);
            }
        }
        else
        {
            // Handle normal item usage
            if (currentPreview != null)
            {
                Destroy(currentPreview.gameObject);
                currentPreview = null;
            }

            if (Input.GetMouseButtonDown(0))
            {
                currentItem.Use(pos);
            }
        }
    }

    public void SetCursorItem(Item item, CursorMode mode)
    {
        currentItem = item;
        currentMode = mode;

        if (item == null)
        {
            // Reset cursor to default
            Cursor.SetCursor(null, Vector2.zero, UnityEngine.CursorMode.Auto);
            return;
        }
        else
        {
            // Set cursor to item icon
            Cursor.SetCursor(item.cursorIcon, Vector2.zero, UnityEngine.CursorMode.ForceSoftware);
        }

        // Clean up preview if switching modes
        if (mode != CursorMode.Support && currentPreview != null)
        {
            Destroy(currentPreview.gameObject);
            currentPreview = null;
        }
    }
}
