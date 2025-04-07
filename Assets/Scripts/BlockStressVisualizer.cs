using TMPro;
using UnityEngine;

// Optional add-on component to show stress values on blocks
[RequireComponent(typeof(Block))]
public class BlockStressVisualizer : MonoBehaviour
{
    public bool showStressValue = true;
    public GameObject stressTextPrefab;

    private Block block;
    private TextMeshPro stressText;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        block = GetComponent<Block>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Create text object for displaying stress
        if (showStressValue && stressTextPrefab != null)
        {
            GameObject textObj = Instantiate(stressTextPrefab, transform);
            textObj.transform.localPosition = new Vector3(0, 0, -0.1f);
            stressText = textObj.GetComponent<TextMeshPro>();
        }
    }

    void Update()
    {
        // Update the visual representation of stress
        if (block != null)
        {
            // Color the block based on stress levels
            if (spriteRenderer != null)
            {
                float stressRatio = Mathf.Clamp01(block.currentStress / block.maxStressCapacity);
                Color stressColor = Color.Lerp(Color.green, Color.red, stressRatio);
                spriteRenderer.color = Color.Lerp(originalColor, stressColor, 0.6f);
            }

            // Update stress text
            if (stressText != null)
            {
                stressText.text = block.currentStress.ToString("F1");

                // Scale text color with stress levels
                float dangerFactor = Mathf.Clamp01(block.currentStress / block.maxStressCapacity);
                stressText.color = Color.Lerp(Color.white, Color.red, dangerFactor);

                // Make text bigger as it approaches failure
                if (dangerFactor > 0.8f)
                {
                    stressText.transform.localScale =
                        Vector3.one * (1 + (dangerFactor - 0.8f) * 1.5f);
                }
                else
                {
                    stressText.transform.localScale = Vector3.one;
                }
            }
        }
    }
}
