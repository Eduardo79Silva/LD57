using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
public class InventoryUISetup : MonoBehaviour
{
    [Header("Prefab Creation")]
    public GameObject slotPrefab;
    public Transform itemSlotContainer;
    public int numberOfSlots = 10;
    public float slotSpacing = 10f;
    public Vector2 slotSize = new(35f, 35f);

    public RectTransform selectionHighlight;

    public void SetupInventoryUI()
    {
        // Clear existing slots
        while (itemSlotContainer.childCount > 0)
        {
            if (itemSlotContainer.GetChild(0) == null)
                break; // Safety check to avoid null reference
            DestroyImmediate(itemSlotContainer.GetChild(0).gameObject);
        }

        // Calculate the total width needed for all slots plus spacing
        float totalWidth = (slotSize.x * numberOfSlots) + (slotSpacing * (numberOfSlots - 1));

        // Update container size to fit all slots perfectly
        RectTransform containerRect = itemSlotContainer as RectTransform;
        if (containerRect != null)
        {
            containerRect.sizeDelta = new Vector2(totalWidth, slotSize.y);
        }

        // Calculate start position (left-most position)
        float startX = -totalWidth / 2f + slotSize.x / 2f;

        // Create new slots
        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, itemSlotContainer);
            slot.name = $"Slot_{i + 1}";

            // Position the slot horizontally
            RectTransform slotRect = slot.GetComponent<RectTransform>();
            slotRect.anchoredPosition = new Vector2(startX + i * (slotSize.x + slotSpacing), 0);
            slotRect.sizeDelta = slotSize;

            // Configure anchors and pivot for correct positioning
            slotRect.anchorMin = new Vector2(0.5f, 0.5f);
            slotRect.anchorMax = new Vector2(0.5f, 0.5f);
            slotRect.pivot = new Vector2(0.5f, 0.5f);

            // Set keyNumber text
            Transform keyNumberTransform = slot.transform.Find("KeyNumber");
            if (keyNumberTransform != null)
            {
                TextMeshProUGUI keyText = keyNumberTransform.GetComponent<TextMeshProUGUI>();
                if (keyText != null)
                {
                    keyText.text = ((i + 1) % 10).ToString();
                }
            }

            // Make sure ItemSlotUI component exists
            ItemSlotUI slotUI = slot.GetComponent<ItemSlotUI>();
            if (slotUI == null)
            {
                slotUI = slot.AddComponent<ItemSlotUI>();
            }

            // Find and assign components
            slotUI.slotBackground = slot.GetComponent<Image>();

            // Find item icon
            Transform iconTransform = slot.transform.Find("ItemIcon");
            if (iconTransform != null)
            {
                slotUI.itemIcon = iconTransform.GetComponent<Image>();
            }

            // Find item name
            Transform nameTransform = slot.transform.Find("ItemName");
            if (nameTransform != null)
            {
                slotUI.itemNameText = nameTransform.GetComponent<TextMeshProUGUI>();
            }

            // Find key number
            Transform keyTransform = slot.transform.Find("KeyNumber");
            if (keyTransform != null)
            {
                slotUI.keyNumberText = keyTransform.GetComponent<TextMeshProUGUI>();
            }

            // Set slot index
            slotUI.slotIndex = i;
            slotUI.keyNumber = (i + 1) % 10;
        }

        // Make sure the InventoryUI component is set up
        InventoryUI inventoryUI = GetComponent<InventoryUI>();
        if (inventoryUI == null)
        {
            inventoryUI = gameObject.AddComponent<InventoryUI>();
        }

        inventoryUI.itemSlotContainer = itemSlotContainer;
        inventoryUI.selectionHighlight = selectionHighlight;
        inventoryUI.itemSlotPrefab = slotPrefab;

        // Setup the selection highlight to be exactly the same size as slots
        if (selectionHighlight != null)
        {
            selectionHighlight.sizeDelta = slotSize;

            // Set proper anchors and pivot for highlight
            selectionHighlight.anchorMin = new Vector2(0.5f, 0.5f);
            selectionHighlight.anchorMax = new Vector2(0.5f, 0.5f);
            selectionHighlight.pivot = new Vector2(0.5f, 0.5f);

            // Initially position it at the first slot
            if (itemSlotContainer.childCount > 0)
            {
                RectTransform firstSlot = itemSlotContainer.GetChild(0) as RectTransform;
                if (firstSlot != null)
                {
                    // Position highlight at the first slot's position
                    selectionHighlight.anchoredPosition = firstSlot.anchoredPosition;
                }
            }
        }
    }
}

[CustomEditor(typeof(InventoryUISetup))]
public class InventoryUISetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        InventoryUISetup setup = (InventoryUISetup)target;

        if (GUILayout.Button("Setup Inventory UI"))
        {
            setup.SetupInventoryUI();
        }
    }
}
#endif
