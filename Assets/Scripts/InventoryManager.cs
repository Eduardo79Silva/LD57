using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public List<Item> itemsPrefabs = new(10);
    public List<Item> items = new(10); // List of items in the inventory
    public int selectedIndex = 0;
    public TextMeshProUGUI ironCountText;
    public TextMeshProUGUI goldCountText;
    public TextMeshProUGUI diamondCountText;

    public int ironCount = 0;
    public int goldCount = 0;
    public int diamondCount = 0;

    // Event that UI can subscribe to
    public delegate void InventoryChangedHandler();
    public event InventoryChangedHandler OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize the inventory with empty slots
        for (int i = 0; i < itemsPrefabs.Count; i++)
        {
            if (itemsPrefabs[i] == null)
            {
                Debug.LogError($"Item prefab at index {i} is null. Please assign a valid prefab.");
                continue;
            }
            items.Add(Instantiate(itemsPrefabs[i]));
            items[i].amount = 0;
        }
        ironCountText.text = "0";
        goldCountText.text = "0";
        diamondCountText.text = "0";
        ironCount = 0;
        goldCount = 0;
        diamondCount = 0;
    }

    public void SelectItem(int index)
    {
        if (index < items.Count)
        {
            selectedIndex = index;
            CursorManager.Instance.SetCursorItem(items[index], items[index].cursorMode);
        }
        else
        {
            // Select empty hand if slot is empty
            selectedIndex = index;
            CursorManager.Instance.SetCursorItem(null, CursorMode.None);
        }
    }

    public Item GetSelectedItem()
    {
        if (selectedIndex < items.Count)
            return items[selectedIndex];
        return null;
    }

    public void UpdateItemCountText(Block block)
    {
        if (block == null)
            return;

        switch (block.oreType)
        {
            case OreType.Iron:
                ironCount += 1;
                break;
            case OreType.Gold:
                goldCount += 1;
                break;
            case OreType.Diamond:
                diamondCount += 1;
                break;
        }

        ironCountText.text = ironCount.ToString();
        goldCountText.text = goldCount.ToString();
        diamondCountText.text = diamondCount.ToString();
    }

    public void AddItem(Item newItem)
    {
        bool itemExists = false;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].GetType() == newItem.GetType())
            {
                Debug.Log($"Adding {newItem.name} to existing stack of {items[i].name}");
                if (items[i].amount + newItem.amount > items[i].maxStackSize)
                {
                    Debug.Log(
                        $"Stack is full, adding {items[i].maxStackSize - items[i].amount} to stack of {items[i].name}"
                    );
                    newItem.amount -= items[i].maxStackSize - items[i].amount;
                    items[i].amount = items[i].maxStackSize;
                }
                else
                {
                    items[i].amount += newItem.amount;
                }
                itemExists = true;
                break;
            }
        }
        if (items.Count < 10 && !itemExists)
        {
            Debug.Log($"Adding new item {newItem.name} to inventory");
            items.Add(newItem);
            // Notify UI
        }
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(Item item)
    {
        //Make copy of item to avoid modifying the original item
        Item itemToRemove = Instantiate(item);
        itemToRemove.itemName = item.itemName;
        itemToRemove.description = item.description;
        itemToRemove.maxStackSize = item.maxStackSize;
        itemToRemove.sellValue = item.sellValue;
        itemToRemove.amount = item.amount;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].GetType() == itemToRemove.GetType())
            {
                items[i].amount -= itemToRemove.amount;
                if (items[i].amount <= 0)
                {
                    items.RemoveAt(i);
                }
                break;
            }
        }
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItemAt(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            items[index].amount--;

            if (items[index].amount <= 0)
            {
                items.RemoveAt(index);
            }

            // If we removed the selected item, update cursor
            if (selectedIndex >= items.Count)
            {
                selectedIndex = Mathf.Max(0, items.Count - 1);
                SelectItem(selectedIndex);
            }

            // Notify UI
            OnInventoryChanged?.Invoke();
        }
    }

    public void ClearInventory()
    {
        items.Clear();
        selectedIndex = 0;
        CursorManager.Instance.SetCursorItem(null, CursorMode.None);
        // Notify UI
        OnInventoryChanged?.Invoke();
    }

    public List<Item> GetInventory()
    {
        return items;
    }
}
