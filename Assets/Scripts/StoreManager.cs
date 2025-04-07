using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance { get; private set; }
    public int playerCoins = 0;
    public List<StoreItem> storeItems;

    public void BuyItem(StoreItem item)
    {
        if (playerCoins >= item.cost)
        {
            InventoryManager.Instance.AddItem(item.item);
            playerCoins -= item.cost;
            // Update UI
        }
    }

    public void SellOre(Ore ore)
    {
        playerCoins += ore.sellValue;
        InventoryManager.Instance.RemoveItem(ore);
        // Update UI
    }
}
