using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

public class SlotPrefabSetup : MonoBehaviour
{
    public Vector2 slotSize = new(70f, 70f);
    public Vector2 iconSize = new(50f, 50f);
    public Vector2 keyNumberSize = new(20f, 20f);
    public Vector2 nameTextSize = new(60f, 20f);

    public void SetupSlotPrefab()
    {
        // Get or add the main RectTransform for the slot
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = gameObject.AddComponent<RectTransform>();
        }
        rectTransform.sizeDelta = slotSize;

        // Get or add the main image component for the slot background
        Image slotBackground = gameObject.GetComponent<Image>();
        if (slotBackground == null)
        {
            slotBackground = gameObject.AddComponent<Image>();
        }
        slotBackground.color = new Color32(0xc9, 0x9f, 0x68, 0xaa);
        // Create an item icon image
        GameObject iconObject = transform.Find("ItemIcon")?.gameObject;
        if (iconObject == null)
        {
            iconObject = new GameObject("ItemIcon");
            iconObject.transform.SetParent(transform);
        }

        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        if (iconRect == null)
        {
            iconRect = iconObject.AddComponent<RectTransform>();
        }
        iconRect.anchoredPosition = Vector2.zero;
        iconRect.sizeDelta = iconSize;
        iconRect.localScale = Vector3.one;

        Image iconImage = iconObject.GetComponent<Image>();
        if (iconImage == null)
        {
            iconImage = iconObject.AddComponent<Image>();
        }
        iconImage.enabled = false; // Start with no image

        // Create key number text
        GameObject keyTextObject = transform.Find("KeyNumber")?.gameObject;
        if (keyTextObject == null)
        {
            keyTextObject = new GameObject("KeyNumber");
            keyTextObject.transform.SetParent(transform);
        }

        RectTransform keyTextRect = keyTextObject.GetComponent<RectTransform>();
        if (keyTextRect == null)
        {
            keyTextRect = keyTextObject.AddComponent<RectTransform>();
        }
        keyTextRect.anchoredPosition = new Vector2(
            -slotSize.x / 2 + keyNumberSize.x / 2,
            slotSize.y / 2 - keyNumberSize.y / 2
        );
        keyTextRect.sizeDelta = keyNumberSize;
        keyTextRect.localScale = Vector3.one;

        TextMeshProUGUI keyText = keyTextObject.GetComponent<TextMeshProUGUI>();
        if (keyText == null)
        {
            keyText = keyTextObject.AddComponent<TextMeshProUGUI>();
        }
        keyText.text = "1";
        keyText.fontSize = 8;
        keyText.alignment = TextAlignmentOptions.Center;
        keyText.color = Color.white;

        // Create item name text (optional)
        GameObject nameTextObject = transform.Find("ItemName")?.gameObject;
        if (nameTextObject == null)
        {
            nameTextObject = new GameObject("ItemName");
            nameTextObject.transform.SetParent(transform);
        }

        RectTransform nameTextRect = nameTextObject.GetComponent<RectTransform>();
        if (nameTextRect == null)
        {
            nameTextRect = nameTextObject.AddComponent<RectTransform>();
        }
        nameTextRect.anchoredPosition = new Vector2(0, -slotSize.y / 2 - nameTextSize.y / 2);
        nameTextRect.sizeDelta = nameTextSize;
        nameTextRect.localScale = Vector3.one;

        TextMeshProUGUI nameText = nameTextObject.GetComponent<TextMeshProUGUI>();
        if (nameText == null)
        {
            nameText = nameTextObject.AddComponent<TextMeshProUGUI>();
        }
        nameText.text = "";
        nameText.fontSize = 6;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = Color.white;
        nameText.enabled = false; // Start with no text

        // Add the ItemSlotUI component
        ItemSlotUI slotUI = gameObject.GetComponent<ItemSlotUI>();
        if (slotUI == null)
        {
            slotUI = gameObject.AddComponent<ItemSlotUI>();
        }
        slotUI.itemIcon = iconImage;
        slotUI.slotBackground = slotBackground;
        slotUI.keyNumberText = keyText;
        slotUI.itemNameText = nameText;
    }
}

[CustomEditor(typeof(SlotPrefabSetup))]
public class SlotPrefabSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SlotPrefabSetup setup = (SlotPrefabSetup)target;

        if (GUILayout.Button("Setup Slot Prefab"))
        {
            setup.SetupSlotPrefab();
        }
    }
}
#endif
