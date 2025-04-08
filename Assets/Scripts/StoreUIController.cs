using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreUIController : MonoBehaviour
{
    [HideInInspector]
    public GameObject storePanel;

    [HideInInspector]
    public TextMeshProUGUI balanceText;

    [HideInInspector]
    public Transform buyItemsContainer;

    [HideInInspector]
    public Transform sellItemsContainer;

    [HideInInspector]
    public GameObject buyItemPrefab;

    [HideInInspector]
    public GameObject sellItemPrefab;

    [HideInInspector]
    public StoreManager storeManager;

    private Item currentItem;
    private CursorMode currentMode;
    private bool needsRefresh = false;

    public void Start()
    {
        UpdateUI();
    }

    public void OpenStore()
    {
        storePanel.SetActive(true);
        currentItem = CursorManager.Instance.currentItem;
        currentMode = CursorManager.Instance.currentMode;
        CursorManager.Instance.SetCursorItem(null, CursorMode.None);
        UpdateUI();
    }

    public void CloseStore()
    {
        storePanel.SetActive(false);
        CursorManager.Instance.SetCursorItem(currentItem, currentMode);
        currentItem = null;
        currentMode = CursorMode.None;
    }

    public void LateUpdate()
    {
        // Process any pending UI updates at the end of the frame
        if (needsRefresh)
        {
            Debug.Log("Refreshing UI in StoreUIController");
            RefreshUIInternal();
            needsRefresh = false;
        }
    }

    // Call this when you need to update the UI
    public void UpdateUI()
    {
        Debug.Log("UpdateUI called in StoreUIController");
        // Flag for refresh but don't do it immediately
        needsRefresh = true;
    }

    // The actual UI refresh logic, called during Update()
    private void RefreshUIInternal()
    {
        if (storeManager != null && storePanel != null && storePanel.activeSelf)
        {
            Debug.Log("Refreshing UI in StoreUIController Internal");
            // Update balance text
            if (balanceText != null)
                balanceText.text = $"Your Balance: {storeManager.playerCoins} DeepCoins";

            // Update buy and sell containers
            SafeRefreshContainer(buyItemsContainer, RefreshBuyItems);
            SafeRefreshContainer(sellItemsContainer, RefreshSellItems);
        }
    }

    private void SafeRefreshContainer(Transform container, System.Action refreshAction)
    {
        if (container == null)
            return;

        Debug.Assert(container != null, "Container is null");
        Debug.Assert(refreshAction != null, "Refresh action is null");

        // Disable the container first to avoid input system interactions
        container.gameObject.SetActive(false);

        // Clear the container
        foreach (Transform child in container)
        {
            // Use DestroyImmediate in editor, Destroy at runtime
            if (Application.isPlaying)
            {
                Debug.Assert(child != null, "Child is null");
                Destroy(child.gameObject);
            }
            else if (child != null)
            {
                if (child.gameObject != buyItemPrefab && child.gameObject != sellItemPrefab)
                    DestroyImmediate(child.gameObject);
            }
        }

        // Now that it's empty, refresh with new content
        refreshAction?.Invoke();

        // Re-enable the container
        container.gameObject.SetActive(true);
    }

    private void RefreshBuyItems()
    {
        if (buyItemPrefab == null || storeManager == null || buyItemsContainer == null)
            return;

        ClearContainer(buyItemsContainer);
        PopulateBuyItems();
    }

    private void RefreshSellItems()
    {
        if (sellItemPrefab == null || storeManager == null || sellItemsContainer == null)
            return;

        // Your sell items population logic here
        // This depends on your inventory system
        ClearContainer(sellItemsContainer);
        PopulateSellItems();
    }

    private void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
        {
            if (child != null)
                Destroy(child.gameObject);
        }
    }

    private void PopulateBuyItems()
    {
        if (buyItemPrefab == null || storeManager == null)
            return;

        foreach (StoreItem item in storeManager.storeItems)
        {
            GameObject itemObj = Instantiate(buyItemPrefab, buyItemsContainer);

            SetupBuyItemUI(itemObj, item);
        }
    }

    private void PopulateSellItems()
    {
        if (sellItemPrefab == null || storeManager == null)
            return;

        // Here you would populate items from inventory that can be sold
        // This would depend on your game's inventory system
        List<Item> itemsToSell = InventoryManager.Instance.GetInventory();
        foreach (Item item in itemsToSell)
        {
            if (item is Ore oreItem && oreItem.sellValue > 0)
            {
                // Only add items that can be sold
                GameObject itemObj = Instantiate(sellItemPrefab, sellItemsContainer);
                SetupSellItemUI(itemObj, item);
            }
        }
    }

    public void ToggleStore()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (storePanel.activeSelf)
            {
                CloseStore();
            }
            else
            {
                OpenStore();
            }
        }
    }

    private void SetupBuyItemUI(GameObject itemObj, StoreItem storeItem)
    {
        Debug.Assert(itemObj != null, "Item object is null");
        Debug.Assert(storeItem != null, "Store item is null");
        Debug.Assert(storeItem.item != null, "Store item has no associated item");
        Debug.Assert(storeItem.item.icon != null, "Store item has no icon");
        Debug.Assert(storeItem.cost > 0, "Store item cost is not valid");

        // Set the item image
        Transform imageTransform = itemObj.transform.Find("ItemImage");
        if (imageTransform != null && storeItem.item != null)
        {
            Image image = imageTransform.GetComponent<Image>();
            if (image != null && storeItem.item.icon != null)
            {
                image.sprite = storeItem.item.icon;
            }
        }

        // Set the item text
        Transform textTransform = itemObj.transform.Find("ItemText");

        if (textTransform != null && storeItem.item != null)
        {
            TextMeshProUGUI text = textTransform.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"{storeItem.item.name} — {storeItem.cost} Coins";
            }
        }

        // Set up buy button click
        Transform buttonTransform = itemObj.transform.Find("BuyButton");
        if (buttonTransform != null)
        {
            Button button = buttonTransform.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    storeManager.BuyItem(storeItem);
                });
            }
        }
    }

    private void SetupSellItemUI(GameObject itemObj, Item storeItem)
    {
        Debug.Assert(itemObj != null, "Item object is null");
        Debug.Assert(storeItem != null, "Item is null");
        Debug.Assert(storeItem.icon != null, "Item has no icon");
        Debug.Assert(storeItem.sellValue > 0, "Item sell value is not valid");
        Debug.Assert(storeItem.amount > 0, "Item amount is not valid");

        // Set the item image
        Transform imageTransform = itemObj.transform.Find("ItemImage");
        if (imageTransform != null && storeItem != null)
        {
            Image image = imageTransform.GetComponent<Image>();
            if (image != null && storeItem.icon != null)
            {
                image.sprite = storeItem.icon;
            }
        }

        Debug.Assert(imageTransform != null, "Image transform is null");

        Debug.Log($"Image transform: {imageTransform}");

        // Set the item text
        Transform textTransform = itemObj.transform.Find("ItemText");
        if (textTransform != null && storeItem != null)
        {
            TextMeshProUGUI text = textTransform.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"{storeItem.name} x{storeItem.amount} — {storeItem.sellValue} Coins";
            }
        }

        Debug.Assert(textTransform != null, "Text transform is null");
        Debug.Log($"Text transform: {textTransform}");
        // Set up sell button click
        Transform buttonTransform = itemObj.transform.Find("SellButton");
        if (buttonTransform != null)
        {
            Button button = buttonTransform.GetComponent<Button>();
            if (button != null)
            {
                try
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() =>
                    {
                        storeManager.SellOre(storeItem as Ore);
                    });
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error removing listeners from Sell button: {e.Message}");
                }
            }
        }
        Debug.Assert(buttonTransform != null, "Button transform is null");
        Debug.Log($"Button transform: {buttonTransform}");

        // Set up sell all button click
        Transform sellAllButtonTransform = itemObj.transform.Find("SellAllButton");
        if (sellAllButtonTransform != null)
        {
            Button sellAllButton = sellAllButtonTransform.GetComponent<Button>();
            if (sellAllButton != null)
            {
                try
                {
                    sellAllButton.onClick.RemoveAllListeners();
                    sellAllButton.onClick.AddListener(() =>
                    {
                        storeManager.SellOre(storeItem as Ore, true);
                    });
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error setting up Sell All button: {e.Message}");
                }
            }
        }
    }
}
