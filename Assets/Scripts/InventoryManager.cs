using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public List<Item> items = new(10);
    public int selectedIndex = 0;

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

    public void AddItem(Item newItem)
    {
        if (items.Count < 10)
        {
            items.Add(newItem);
            // Notify UI
            OnInventoryChanged?.Invoke();
        }
    }

    public void RemoveItem(Item item)
    {
        if (items.Remove(item))
        {
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

    public void RemoveItemAt(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            items.RemoveAt(index);

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
}
