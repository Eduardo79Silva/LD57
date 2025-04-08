using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
public class StoreUISetup : MonoBehaviour
{
    [Header("Store UI Settings")]
    public Color textColor = Color.white;

    [Header("Store Panel Settings")]
    public Vector2 storePanelSize = new Vector2(600f, 500f);
    public Color panelBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);

    [Header("Title Settings")]
    public string storeTitle = "THE DEEP MARKET";
    public float titleHeight = 50f;
    public Color titleBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);

    [Header("Balance Settings")]
    public float balanceHeight = 40f;
    public Color balanceBackgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);

    [Header("Content Settings")]
    public float sectionTitleHeight = 30f;
    public Color buyBackgroundColor = new Color(0.12f, 0.18f, 0.12f, 0.8f);
    public Color sellBackgroundColor = new Color(0.18f, 0.12f, 0.12f, 0.8f);

    [Header("Prefabs")]
    public GameObject buyItemPrefab;
    public GameObject sellItemPrefab;

    [Header("References")]
    public StoreManager storeManager;

    private GameObject storePanel;
    private TextMeshProUGUI balanceText;
    private Transform buyItemsContainer;
    private Transform sellItemsContainer;
    private StoreUIController uiController;

    public void SetupStoreUI()
    {
        // First, clean up any existing UI
        CleanupExistingUI();

        // Create canvas if needed
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10; // Put it in front of other UI

            // Add canvas scaler
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            // Add GraphicRaycaster
            gameObject.AddComponent<GraphicRaycaster>();
        }

        // Create the entire store UI hierarchy
        CreateStorePanel();
        CreateStoreHeader();
        CreateBalanceDisplay();
        CreateContentArea();

        // Set up the controller script
        SetupStoreUIController();
    }

    private void CleanupExistingUI()
    {
        // Find and destroy any existing store panel
        Transform existingPanel = transform.Find("StorePanel");
        if (existingPanel != null)
        {
            DestroyImmediate(existingPanel.gameObject);
        }

        // Remove existing controller if present
        StoreUIController existingController = GetComponent<StoreUIController>();
        if (existingController != null)
        {
            DestroyImmediate(existingController);
        }
    }

    private void CreateStorePanel()
    {
        // Create panel GameObject
        storePanel = new GameObject("StorePanel");
        storePanel.transform.SetParent(transform, false);

        // Add RectTransform
        RectTransform panelRect = storePanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = storePanelSize;
        panelRect.anchoredPosition = Vector2.zero;

        // Add panel image (background)
        Image panelImage = storePanel.AddComponent<Image>();
        panelImage.color = panelBackgroundColor;
        panelImage.raycastTarget = true;

        // Start with panel inactive (will be activated by controller)
        storePanel.SetActive(false);
    }

    private void CreateStoreHeader()
    {
        // Create header GameObject
        GameObject header = new GameObject("Header");
        header.transform.SetParent(storePanel.transform, false);

        // Set up RectTransform
        RectTransform headerRect = header.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1f);
        headerRect.sizeDelta = new Vector2(0, titleHeight);
        headerRect.anchoredPosition = Vector2.zero;

        // Add background image
        Image headerImage = header.AddComponent<Image>();
        headerImage.color = titleBackgroundColor;

        // Create title text
        GameObject titleTextObj = new GameObject("TitleText");
        titleTextObj.transform.SetParent(header.transform, false);

        RectTransform titleRect = titleTextObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(10, 0);
        titleRect.offsetMax = new Vector2(-50, 0);

        TextMeshProUGUI titleText = titleTextObj.AddComponent<TextMeshProUGUI>();
        titleText.text = storeTitle;
        titleText.fontSize = 24;
        titleText.alignment = TextAlignmentOptions.Left;
        titleText.color = textColor;

        // Create close button
        GameObject closeBtn = new GameObject("CloseButton");
        closeBtn.transform.SetParent(header.transform, false);

        RectTransform closeBtnRect = closeBtn.AddComponent<RectTransform>();
        closeBtnRect.anchorMin = new Vector2(1, 0.5f);
        closeBtnRect.anchorMax = new Vector2(1, 0.5f);
        closeBtnRect.pivot = new Vector2(1, 0.5f);
        closeBtnRect.sizeDelta = new Vector2(40, 40);
        closeBtnRect.anchoredPosition = new Vector2(-10, 0);

        Image closeBtnImage = closeBtn.AddComponent<Image>();
        closeBtnImage.color = new Color(0.6f, 0f, 0f);

        CloseButtonHandler closeHandler = closeBtn.AddComponent<CloseButtonHandler>();

        Button closeButton = closeBtn.AddComponent<Button>();
        closeButton.targetGraphic = closeBtnImage;

        closeButton.onClick.RemoveAllListeners(); // Clear any previous listeners
        closeButton.onClick.AddListener(() =>
        {
            Debug.Log("Close button clicked!");
            if (uiController != null)
            {
                uiController.CloseStore();
            }
            else
            {
                Debug.LogError("UI Controller is null!");
            }
        });

        // Create X text for close button
        GameObject closeTextObj = new GameObject("CloseText");
        closeTextObj.transform.SetParent(closeBtn.transform, false);

        RectTransform closeTextRect = closeTextObj.AddComponent<RectTransform>();
        closeTextRect.anchorMin = new Vector2(0, 0);
        closeTextRect.anchorMax = new Vector2(1, 1);
        closeTextRect.offsetMin = Vector2.zero;
        closeTextRect.offsetMax = Vector2.zero;

        TextMeshProUGUI closeText = closeTextObj.AddComponent<TextMeshProUGUI>();
        closeText.text = "X";
        closeText.fontSize = 24;
        closeText.alignment = TextAlignmentOptions.Center;
        closeText.color = textColor;
    }

    private void CreateBalanceDisplay()
    {
        // Create balance display GameObject
        GameObject balanceDisplay = new GameObject("BalanceDisplay");
        balanceDisplay.transform.SetParent(storePanel.transform, false);

        // Set up RectTransform
        RectTransform balanceRect = balanceDisplay.AddComponent<RectTransform>();
        balanceRect.anchorMin = new Vector2(0, 1);
        balanceRect.anchorMax = new Vector2(1, 1);
        balanceRect.pivot = new Vector2(0.5f, 1f);
        balanceRect.sizeDelta = new Vector2(0, balanceHeight);
        balanceRect.anchoredPosition = new Vector2(0, -titleHeight);

        // Add background image
        Image balanceImage = balanceDisplay.AddComponent<Image>();
        balanceImage.color = balanceBackgroundColor;

        // Create balance text
        GameObject balanceTextObj = new GameObject("BalanceText");
        balanceTextObj.transform.SetParent(balanceDisplay.transform, false);

        RectTransform balanceTextRect = balanceTextObj.AddComponent<RectTransform>();
        balanceTextRect.anchorMin = new Vector2(0, 0);
        balanceTextRect.anchorMax = new Vector2(1, 1);
        balanceTextRect.offsetMin = new Vector2(10, 0);
        balanceTextRect.offsetMax = new Vector2(-10, 0);

        balanceText = balanceTextObj.AddComponent<TextMeshProUGUI>();
        balanceText.text = "Your Balance: 0 ScrapCoins";
        balanceText.fontSize = 20;
        balanceText.alignment = TextAlignmentOptions.Left;
        balanceText.color = textColor;
    }

    private void CreateContentArea()
    {
        // Calculate the height of the content area
        float topOffset = titleHeight + balanceHeight;
        float contentHeight = storePanelSize.y - topOffset;
        float sectionHeight = contentHeight / 2;

        // Create content area GameObject
        GameObject contentArea = new GameObject("ContentArea");
        contentArea.transform.SetParent(storePanel.transform, false);

        // Set up RectTransform
        RectTransform contentRect = contentArea.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.offsetMin = new Vector2(0, 0);
        contentRect.offsetMax = new Vector2(0, -topOffset);

        // Create Buy Section
        GameObject buySection = new GameObject("BuySection");
        buySection.transform.SetParent(contentArea.transform, false);

        RectTransform buyRect = buySection.AddComponent<RectTransform>();
        buyRect.anchorMin = new Vector2(0, 0.5f);
        buyRect.anchorMax = new Vector2(1, 1);
        buyRect.offsetMin = new Vector2(10, 10);
        buyRect.offsetMax = new Vector2(-10, -10);

        Image buyBgImage = buySection.AddComponent<Image>();
        buyBgImage.color = buyBackgroundColor;

        // Create Buy Section Title
        GameObject buySectionTitle = new GameObject("SectionTitle");
        buySectionTitle.transform.SetParent(buySection.transform, false);

        RectTransform buySectionTitleRect = buySectionTitle.AddComponent<RectTransform>();
        buySectionTitleRect.anchorMin = new Vector2(0, 1);
        buySectionTitleRect.anchorMax = new Vector2(1, 1);
        buySectionTitleRect.sizeDelta = new Vector2(0, sectionTitleHeight);
        buySectionTitleRect.anchoredPosition = new Vector2(0, -sectionTitleHeight / 2);

        TextMeshProUGUI buySectionTitleText = buySectionTitle.AddComponent<TextMeshProUGUI>();
        buySectionTitleText.text = "BUY";
        buySectionTitleText.fontSize = 18;
        buySectionTitleText.alignment = TextAlignmentOptions.Center;
        buySectionTitleText.color = textColor;

        // Create Buy Items ScrollView
        GameObject buyScrollView = new GameObject("BuyItemsScrollView");
        buyScrollView.transform.SetParent(buySection.transform, false);

        RectTransform buyScrollViewRect = buyScrollView.AddComponent<RectTransform>();
        buyScrollViewRect.anchorMin = new Vector2(0, 0);
        buyScrollViewRect.anchorMax = new Vector2(1, 1);
        buyScrollViewRect.offsetMin = new Vector2(5, 5);
        buyScrollViewRect.offsetMax = new Vector2(-5, -sectionTitleHeight - 5);

        ScrollRect buyScrollRect = buyScrollView.AddComponent<ScrollRect>();

        // Create Viewport for buy items
        GameObject buyViewport = new GameObject("Viewport");
        buyViewport.transform.SetParent(buyScrollView.transform, false);

        RectTransform buyViewportRect = buyViewport.AddComponent<RectTransform>();
        buyViewportRect.anchorMin = Vector2.zero;
        buyViewportRect.anchorMax = Vector2.one;
        buyViewportRect.sizeDelta = Vector2.zero;
        buyViewportRect.anchoredPosition = Vector2.zero;

        Image buyViewportImage = buyViewport.AddComponent<Image>();
        buyViewportImage.color = new Color(1, 1, 1, 0.05f);

        // Add mask component
        Mask buyViewportMask = buyViewport.AddComponent<Mask>();
        buyViewportMask.showMaskGraphic = true;

        // Create Content for buy items (container)
        GameObject buyItemsContainerObj = new GameObject("BuyItemsContainer");
        buyItemsContainerObj.transform.SetParent(buyViewport.transform, false);

        RectTransform buyItemsContainerRect = buyItemsContainerObj.AddComponent<RectTransform>();
        buyItemsContainerRect.anchorMin = new Vector2(0, 1);
        buyItemsContainerRect.anchorMax = new Vector2(1, 1);
        buyItemsContainerRect.pivot = new Vector2(0f, 1);
        buyItemsContainerRect.sizeDelta = new Vector2(0, 0);

        HorizontalLayoutGroup buyLayoutGroup =
            buyItemsContainerObj.AddComponent<HorizontalLayoutGroup>();
        buyLayoutGroup.padding = new RectOffset(10, 10, 10, 10);
        buyLayoutGroup.spacing = 10;
        buyLayoutGroup.childAlignment = TextAnchor.UpperCenter;
        buyLayoutGroup.childControlWidth = true;
        buyLayoutGroup.childControlHeight = false;
        buyLayoutGroup.childForceExpandWidth = true;
        buyLayoutGroup.childForceExpandHeight = false;

        ContentSizeFitter buySizeFitter = buyItemsContainerObj.AddComponent<ContentSizeFitter>();
        buySizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
        buySizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;

        buyItemsContainer = buyItemsContainerObj.transform;

        // Set up the scroll rect
        buyScrollRect.content = buyItemsContainerRect;
        buyScrollRect.viewport = buyViewportRect;
        buyScrollRect.horizontal = true;
        buyScrollRect.vertical = false;
        buyScrollRect.scrollSensitivity = 5;
        buyScrollRect.movementType = ScrollRect.MovementType.Elastic;
        buyScrollRect.elasticity = 0.1f;
        buyScrollRect.inertia = true;
        buyScrollRect.decelerationRate = 0.5f;

        // Create Sell Section (very similar structure)
        GameObject sellSection = new GameObject("SellSection");
        sellSection.transform.SetParent(contentArea.transform, false);

        RectTransform sellRect = sellSection.AddComponent<RectTransform>();
        sellRect.anchorMin = new Vector2(0, 0);
        sellRect.anchorMax = new Vector2(1, 0.5f);
        sellRect.offsetMin = new Vector2(10, 10);
        sellRect.offsetMax = new Vector2(-10, -10);

        Image sellBgImage = sellSection.AddComponent<Image>();
        sellBgImage.color = sellBackgroundColor;

        // Create Sell Section Title
        GameObject sellSectionTitle = new GameObject("SectionTitle");
        sellSectionTitle.transform.SetParent(sellSection.transform, false);

        RectTransform sellSectionTitleRect = sellSectionTitle.AddComponent<RectTransform>();
        sellSectionTitleRect.anchorMin = new Vector2(0, 1);
        sellSectionTitleRect.anchorMax = new Vector2(1, 1);
        sellSectionTitleRect.sizeDelta = new Vector2(0, sectionTitleHeight);
        sellSectionTitleRect.anchoredPosition = new Vector2(0, -sectionTitleHeight / 2);

        TextMeshProUGUI sellSectionTitleText = sellSectionTitle.AddComponent<TextMeshProUGUI>();
        sellSectionTitleText.text = "SELL";
        sellSectionTitleText.fontSize = 18;
        sellSectionTitleText.alignment = TextAlignmentOptions.Center;
        sellSectionTitleText.color = textColor;

        // Create Sell Items ScrollView
        GameObject sellScrollView = new GameObject("SellItemsScrollView");
        sellScrollView.transform.SetParent(sellSection.transform, false);

        RectTransform sellScrollViewRect = sellScrollView.AddComponent<RectTransform>();
        sellScrollViewRect.anchorMin = new Vector2(0, 0);
        sellScrollViewRect.anchorMax = new Vector2(1, 1);
        sellScrollViewRect.offsetMin = new Vector2(5, 5);
        sellScrollViewRect.offsetMax = new Vector2(-5, -sectionTitleHeight - 5);

        ScrollRect sellScrollRect = sellScrollView.AddComponent<ScrollRect>();

        // Create Viewport for sell items
        GameObject sellViewport = new GameObject("Viewport");
        sellViewport.transform.SetParent(sellScrollView.transform, false);

        RectTransform sellViewportRect = sellViewport.AddComponent<RectTransform>();
        sellViewportRect.anchorMin = Vector2.zero;
        sellViewportRect.anchorMax = Vector2.one;
        sellViewportRect.sizeDelta = Vector2.zero;
        sellViewportRect.anchoredPosition = Vector2.zero;

        Image sellViewportImage = sellViewport.AddComponent<Image>();
        sellViewportImage.color = new Color(1, 1, 1, 0.05f);

        // Add mask component
        Mask sellViewportMask = sellViewport.AddComponent<Mask>();
        sellViewportMask.showMaskGraphic = true;

        // Create Content for sell items (container)
        GameObject sellItemsContainerObj = new GameObject("SellItemsContainer");
        sellItemsContainerObj.transform.SetParent(sellViewport.transform, false);

        RectTransform sellItemsContainerRect = sellItemsContainerObj.AddComponent<RectTransform>();
        sellItemsContainerRect.anchorMin = new Vector2(0, 1);
        sellItemsContainerRect.anchorMax = new Vector2(1, 1);
        sellItemsContainerRect.pivot = new Vector2(0f, 1);
        sellItemsContainerRect.sizeDelta = new Vector2(0, 0);

        HorizontalLayoutGroup sellLayoutGroup =
            sellItemsContainerObj.AddComponent<HorizontalLayoutGroup>();
        sellLayoutGroup.padding = new RectOffset(10, 10, 10, 10);
        sellLayoutGroup.spacing = 10;
        sellLayoutGroup.childAlignment = TextAnchor.UpperCenter;
        sellLayoutGroup.childControlWidth = true;
        sellLayoutGroup.childControlHeight = false;
        sellLayoutGroup.childForceExpandWidth = true;
        sellLayoutGroup.childForceExpandHeight = false;

        ContentSizeFitter sellSizeFitter = sellItemsContainerObj.AddComponent<ContentSizeFitter>();
        sellSizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
        sellSizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;

        sellItemsContainer = sellItemsContainerObj.transform;

        // Set up the scroll rect
        sellScrollRect.content = sellItemsContainerRect;
        sellScrollRect.viewport = sellViewportRect;
        sellScrollRect.horizontal = true;
        sellScrollRect.vertical = false;
        sellScrollRect.scrollSensitivity = 5;
        sellScrollRect.movementType = ScrollRect.MovementType.Elastic;
        sellScrollRect.elasticity = 0.1f;
        sellScrollRect.inertia = true;
        sellScrollRect.decelerationRate = 0.5f;
    }

    private void SetupStoreUIController()
    {
        // Add StoreUIController component
        uiController = gameObject.AddComponent<StoreUIController>();

        // Assign references
        uiController.storeManager = storeManager;
        uiController.storePanel = storePanel;
        uiController.balanceText = balanceText;
        uiController.buyItemsContainer = buyItemsContainer;
        uiController.sellItemsContainer = sellItemsContainer;
        uiController.buyItemPrefab = buyItemPrefab;
        uiController.sellItemPrefab = sellItemPrefab;

        // Connect close button - with more robust error checking
        Transform headerTransform = storePanel.transform.Find("Header");
        if (headerTransform == null)
        {
            Debug.LogError("Header not found in storePanel!");
            return;
        }

        // Connect close button action
        Transform closeButtonTransform = headerTransform.Find("CloseButton");
        if (closeButtonTransform == null)
        {
            Debug.LogError("CloseButton not found in Header!");
            return;
        }

        Button closeButton = closeButtonTransform.GetComponent<Button>();
        if (closeButton == null)
        {
            Debug.LogError("Button component not found on CloseButton!");
            return;
        }

        // Remove any existing listeners and add the new one
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() =>
        {
            Debug.Log("Close button clicked!");
            uiController.CloseStore();
        });

        // Update StoreManager reference
        if (storeManager != null)
        {
            storeManager.uiController = uiController;
        }
        else
        {
            Debug.LogWarning("StoreManager reference not set in StoreUISetup!");
        }
    }
}

[CustomEditor(typeof(StoreUISetup))]
public class StoreUISetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        StoreUISetup setup = (StoreUISetup)target;

        if (GUILayout.Button("Setup Store UI"))
            setup.SetupStoreUI();
    }
}

#endif
