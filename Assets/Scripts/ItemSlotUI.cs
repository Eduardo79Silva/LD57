using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Components")]
    public Image itemIcon;
    public Image slotBackground;
    public TextMeshProUGUI keyNumberText;
    public TextMeshProUGUI itemNameText;

    [HideInInspector]
    public int slotIndex;

    [HideInInspector]
    public int keyNumber;

    [HideInInspector]
    public InventoryUI inventoryUI;

    private Item currentItem;

    public void UpdateItem(Item item)
    {
        currentItem = item;

        if (item != null)
        {
            // Show item icon
            if (itemIcon != null)
            {
                itemIcon.sprite = item.icon;
                itemIcon.enabled = true;
            }

            // Show item name if we have that element
            if (itemNameText != null)
            {
                itemNameText.text = item.itemName;
                itemNameText.enabled = true;
            }
        }
        else
        {
            // Hide icon for empty slot
            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }

            // Hide name text
            if (itemNameText != null)
            {
                itemNameText.text = "";
                itemNameText.enabled = false;
            }
        }
    }

    public void UpdateSelectionState(bool isSelected)
    {
        if (slotBackground != null && inventoryUI != null)
        {
            Color targetColor = isSelected
                ? inventoryUI.selectedColor
                : inventoryUI.unselectedColor;

            slotBackground.color = targetColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Select this slot when clicked
        if (inventoryUI != null)
        {
            inventoryUI.SelectSlot(slotIndex);
        }
    }
}
