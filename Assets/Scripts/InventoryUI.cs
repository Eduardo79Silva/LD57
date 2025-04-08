using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject itemSlotPrefab;
    public Transform itemSlotContainer;
    public RectTransform selectionHighlight;

    [Header("Layout Settings")]
    public int maxSlots = 10;
    public Vector2 slotSize = new(70f, 70f);
    public float slotSpacing = 10f;
    public Color selectedColor = new(0xe7, 0xd1, 0x88, 1f);
    public Color unselectedColor = new(0xc9, 0x9f, 0x68, 1f);

    private List<ItemSlotUI> itemSlots = new();
    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = InventoryManager.Instance;
        // Subscribe to inventory changes
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged += UpdateAllSlots;
        }

        SetupInventoryUI();
        UpdateAllSlots();
        SelectSlot(0); // Select first slot by default
    }

    void OnDestroy()
    {
        // Unsubscribe from inventory changes
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged -= UpdateAllSlots;
        }
    }

    void Update()
    {
        //// Handle number keys 1-9, 0 (for slot 10)
        //for (int i = 0; i < 10; i++)
        //{
        //    int keyNumber = (i + 1) % 10; // Maps 0-9 to 1-9,0
        //    if (Input.GetKeyDown(keyNumber.ToString()))
        //    {
        //        SelectSlot(i);
        //    }
        //}
    }

    public void SetupInventoryUI()
    {
        // Clear any existing slots
        foreach (Transform child in itemSlotContainer)
        {
            if (child.gameObject != itemSlotPrefab && child.gameObject != null)
                Destroy(child.gameObject);
        }
        itemSlots.Clear();

        // Calculate container dimensions
        float totalWidth = (slotSize.x * maxSlots) + (slotSpacing * (maxSlots - 1));

        // Update container size
        RectTransform containerRect = itemSlotContainer as RectTransform;
        if (containerRect != null)
        {
            containerRect.sizeDelta = new Vector2(totalWidth, slotSize.y);
        }

        // Calculate starting position (leftmost position)
        float startX = -(totalWidth / 2) + (slotSize.x / 2);

        // Create slots with proper horizontal layout
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slotObject = Instantiate(itemSlotPrefab, itemSlotContainer);
            slotObject.name = $"Slot_{i + 1}";

            // Position the slot
            RectTransform slotRect = slotObject.GetComponent<RectTransform>();
            slotRect.anchoredPosition = new Vector2(startX + i * (slotSize.x + slotSpacing), 0);
            slotRect.sizeDelta = slotSize;

            // Configure proper anchoring
            slotRect.anchorMin = new Vector2(0.5f, 0.5f);
            slotRect.anchorMax = new Vector2(0.5f, 0.5f);
            slotRect.pivot = new Vector2(0.5f, 0.5f);

            // Set up the slot
            ItemSlotUI slotUI = slotObject.GetComponent<ItemSlotUI>();
            if (slotUI != null)
            {
                slotUI.slotIndex = i;
                slotUI.keyNumber = (i + 1) % 10; // 1-9, 0
                slotUI.inventoryUI = this;

                // Update key number text
                if (slotUI.keyNumberText != null)
                {
                    slotUI.keyNumberText.text = slotUI.keyNumber.ToString();
                }

                itemSlots.Add(slotUI);
            }
        }

        // Set up highlight
        if (selectionHighlight != null)
        {
            selectionHighlight.sizeDelta = slotSize;
            selectionHighlight.anchorMin = new Vector2(0.5f, 0.5f);
            selectionHighlight.anchorMax = new Vector2(0.5f, 0.5f);
            selectionHighlight.pivot = new Vector2(0.5f, 0.5f);
        }
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= maxSlots || inventoryManager == null)
            return;

        inventoryManager.SelectItem(index);

        // Update selection highlight position
        if (selectionHighlight != null && index < itemSlots.Count)
        {
            selectionHighlight.position = itemSlots[index].transform.position;
        }

        // Update all slot visuals
        for (int i = 0; i < itemSlots.Count; i++)
        {
            itemSlots[i].UpdateSelectionState(i == index);
        }
    }

    public void UpdateAllSlots()
    {
        Debug.Log("Updating all slots in inventory UI");
        if (inventoryManager == null)
            return;

        for (int i = 0; i < maxSlots; i++)
        {
            if (i < itemSlots.Count)
            {
                bool hasItem = i < inventoryManager.items.Count;
                Item item = hasItem ? inventoryManager.items[i] : null;
                itemSlots[i].UpdateItem(item);
            }
        }
        Debug.Log("All slots updated successfully.");
    }

    public void CloseInventory()
    {
        gameObject.SetActive(false);
    }

    public void OpenInventory()
    {
        gameObject.SetActive(true);
    }

    public void ToggleInventory()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
