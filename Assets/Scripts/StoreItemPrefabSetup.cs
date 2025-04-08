using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
public class StoreItemPrefabSetup : MonoBehaviour
{
    public enum PrefabType
    {
        BuyItem,
        SellItem,
    }

    [Header("Prefab Type")]
    public PrefabType prefabType = PrefabType.BuyItem;

    [Header("Item Settings")]
    public Vector2 itemSize = new Vector2(0, 40f);
    public Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);

    [Header("Image Settings")]
    public Vector2 imageSize = new Vector2(40f, 40f);

    [Header("Text Settings")]
    public float fontSize = 14f;
    public Color textColor = Color.white;

    [Header("Button Settings")]
    public Vector2 buttonSize = new Vector2(80f, 30f);
    public Color buyButtonColor = new Color(0.2f, 0.6f, 0.2f);
    public Color sellButtonColor = new Color(0.6f, 0.2f, 0.2f);

    public void SetupPrefab()
    {
        // Setup base item container
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = gameObject.AddComponent<RectTransform>();
        }
        rectTransform.sizeDelta = itemSize;

        // Add background image
        Image backgroundImage = GetComponent<Image>();
        if (backgroundImage == null)
        {
            backgroundImage = gameObject.AddComponent<Image>();
        }
        backgroundImage.color = backgroundColor;

        // Create horizontal layout
        HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
        }
        layoutGroup.padding = new RectOffset(10, 10, 5, 5);
        layoutGroup.spacing = 10;
        layoutGroup.childAlignment = TextAnchor.MiddleLeft;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;

        // Create image component
        GameObject itemImageObj = transform.Find("ItemImage")?.gameObject;
        if (itemImageObj == null)
        {
            itemImageObj = new GameObject("ItemImage");
            itemImageObj.transform.SetParent(transform, false);
        }

        RectTransform imageRect = itemImageObj.GetComponent<RectTransform>();
        if (imageRect == null)
        {
            imageRect = itemImageObj.AddComponent<RectTransform>();
        }
        imageRect.sizeDelta = imageSize;

        Image itemImage = itemImageObj.GetComponent<Image>();
        if (itemImage == null)
        {
            itemImage = itemImageObj.AddComponent<Image>();
        }
        itemImage.preserveAspect = true;

        // Create button(s) based on prefab type
        if (prefabType == PrefabType.BuyItem)
        {
            // Create Buy Button
            Debug.Assert(buyButtonColor != null, "Buy button color is null");
            Debug.Log($"Buy button color: {buyButtonColor}");
            CreateButton("BuyButton", "Buy", buyButtonColor);
        }
        else
        {
            Debug.Assert(sellButtonColor != null, "Sell button color is null");
            Debug.Log($"Sell button color: {sellButtonColor}");
            // Create Sell and Sell All buttons
            CreateButton("SellButton", "Sell", sellButtonColor);
            CreateButton("SellAllButton", "Sell All", sellButtonColor);
        }

        // Create text component
        GameObject textObj = transform.Find("ItemText")?.gameObject;
        if (textObj == null)
        {
            textObj = new GameObject("ItemText");
            textObj.transform.SetParent(transform, false);
        }

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        if (textRect == null)
        {
            textRect = textObj.AddComponent<RectTransform>();
        }
        //Place text below the image
        textRect.sizeDelta = new Vector2(50, 0);

        TextMeshProUGUI itemText = textObj.GetComponent<TextMeshProUGUI>();
        if (itemText == null)
        {
            itemText = textObj.AddComponent<TextMeshProUGUI>();
        }
        itemText.text =
            prefabType == PrefabType.BuyItem ? "Item Name — 10 Coins" : "Item Name x2 — 20 Coins";
        itemText.fontSize = fontSize;
        itemText.color = textColor;
        itemText.alignment = TextAlignmentOptions.MidlineLeft;
        // Add a layout element to text so it fills available space
        LayoutElement textLayoutElement = textObj.GetComponent<LayoutElement>();
        if (textLayoutElement == null)
        {
            textLayoutElement = textObj.AddComponent<LayoutElement>();
        }
        textLayoutElement.flexibleWidth = 1;
    }

    private GameObject CreateButton(string name, string text, Color buttonColor)
    {
        GameObject buttonObj = transform.Find(name)?.gameObject;
        if (buttonObj == null)
        {
            buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(transform, false);
        }

        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        if (buttonRect == null)
        {
            buttonRect = buttonObj.AddComponent<RectTransform>();
        }
        buttonRect.sizeDelta = buttonSize;

        Image buttonImage = buttonObj.GetComponent<Image>();
        if (buttonImage == null)
        {
            buttonImage = buttonObj.AddComponent<Image>();
        }
        buttonImage.color = buttonColor;

        Button button = buttonObj.GetComponent<Button>();
        if (button == null)
        {
            button = buttonObj.AddComponent<Button>();
        }

        // Set up button visual states
        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        colors.highlightedColor = new Color(
            Mathf.Min(buttonColor.r + 0.1f, 1f),
            Mathf.Min(buttonColor.g + 0.1f, 1f),
            Mathf.Min(buttonColor.b + 0.1f, 1f),
            buttonColor.a
        );
        colors.pressedColor = new Color(
            Mathf.Max(buttonColor.r - 0.1f, 0f),
            Mathf.Max(buttonColor.g - 0.1f, 0f),
            Mathf.Max(buttonColor.b - 0.1f, 0f),
            buttonColor.a
        );
        colors.disabledColor = new Color(
            buttonColor.r * 0.6f,
            buttonColor.g * 0.6f,
            buttonColor.b * 0.6f,
            buttonColor.a
        );
        button.colors = colors;

        // Create button text
        GameObject buttonTextObj = buttonObj.transform.Find("ButtonText")?.gameObject;
        if (buttonTextObj == null)
        {
            buttonTextObj = new GameObject("ButtonText");
            buttonTextObj.transform.SetParent(buttonObj.transform, false);
        }

        RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
        if (buttonTextRect == null)
        {
            buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
        }
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;

        TextMeshProUGUI buttonText = buttonTextObj.GetComponent<TextMeshProUGUI>();
        if (buttonText == null)
        {
            buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        }
        buttonText.text = text;
        buttonText.fontSize = fontSize - 2;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;

        return buttonObj;
    }
}

[CustomEditor(typeof(StoreItemPrefabSetup))]
public class StoreItemPrefabSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        StoreItemPrefabSetup setup = (StoreItemPrefabSetup)target;

        if (GUILayout.Button("Setup Prefab"))
        {
            setup.SetupPrefab();
        }
    }
}

#endif
