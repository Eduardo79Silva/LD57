using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance { get; private set; }
    public int playerCoins = 0;
    public List<StoreItem> storeItems;

    // Reference to the UI controller
    [HideInInspector]
    public StoreUIController uiController;

    // Event handler for when the store is opened or closed
    public delegate void StoreOpenedHandler();

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void BuyItem(StoreItem item)
    {
        if (playerCoins >= item.cost)
        {
            Debug.Log($"Bought {item.item.name} for {item.cost} coins.");
            InventoryManager.Instance.AddItem(item.item);
            Debug.Log($"Added {item.item.name} to inventory.");
            playerCoins -= item.cost;

            // Update UI
            if (uiController != null)
            {
                Debug.Log("Updating UI after purchase.");
                uiController.UpdateUI();
            }
        }
    }

    public void SellOre(Ore ore, bool sellAll = false)
    {
        Debug.Log($"Selling ore: {ore.name} for {ore.sellValue} coins.");
        if (uiController == null)
        {
            Debug.LogError("UI Controller is not set. Cannot update UI.");
            return;
        }
        if (ore == null || ore.sellValue <= 0)
            return;

        // Sell all ores of the same type
        foreach (var item in InventoryManager.Instance.items)
        {
            if (item is Ore oreItem && oreItem.type == ore.type)
            {
                playerCoins += oreItem.sellValue * oreItem.amount;
                // Create copy of ore to remove
                Item oreCopy = Instantiate(ore);

                if (sellAll)
                {
                    oreCopy.amount = oreItem.amount;
                    InventoryManager.Instance.RemoveItem(oreCopy);
                }
                else
                {
                    oreCopy.amount = 1; // Sell only one ore
                    InventoryManager.Instance.RemoveItem(oreCopy);
                }
                Debug.Log($"Removed {oreCopy.amount} of {oreItem.name} from inventory.");
                break;
            }
        }

        if (uiController != null)
        {
            Debug.Log("Updating UI after ores.");
            uiController.UpdateUI();
        }
    }

    private void SellSingleOre(Ore ore)
    {
        if (ore == null || ore.sellValue <= 0)
            return;

        InventoryManager.Instance.RemoveItem(ore);
        playerCoins += ore.sellValue;
    }

    // Method to open the store
    public void OpenStore()
    {
        if (uiController != null)
        {
            uiController.OpenStore();
        }
    }

    // Method to close the store
    public void CloseStore()
    {
        if (uiController != null)
        {
            uiController.CloseStore();
        }
    }

    // Method to toggle the store
    public void ToggleStoreInput()
    {
        if (uiController != null)
        {
            uiController.ToggleStore();
        }
    }

    public void Update()
    {
        // ToggleStoreInput(); // FIXME: Uncomment this line if you want to toggle the store with a key press
    }
}
