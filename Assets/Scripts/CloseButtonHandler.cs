using UnityEngine;
using UnityEngine.UI;

public class CloseButtonHandler : MonoBehaviour
{
    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    public void OnClick()
    {

        // Find the StoreUIController in the scene
        StoreUIController controller = FindFirstObjectByType<StoreUIController>();
        if (controller != null)
        {
            controller.CloseStore();
        }
        else
        {

            // Fallback: Try to find and close parent panel
            Transform panel = transform.parent.parent; // Button → Header → Panel
            if (panel != null)
            {
                panel.gameObject.SetActive(false);
            }
        }
    }
}
