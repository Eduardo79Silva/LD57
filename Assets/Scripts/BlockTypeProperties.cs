using UnityEngine;

// This script should be attached to each block prefab to define its physical properties
public class BlockTypeProperties : MonoBehaviour
{
    [Header("Physical Properties")]
    public float weight = 1.0f; // How heavy this block is
    public float maxStressCapacity = 10.0f; // How much stress this block can handle
    public float lateralConnectionStrength = 0.7f; // Connection strength to blocks on same level
    public float diagonalConnectionStrength = 0.3f; // Connection strength to diagonal blocks

    [Header("Material Properties")]
    public bool isBrittle = false; // Brittle materials break more easily under tension
    public float dampingFactor = 0.0f; // How much this material absorbs/reduces force transfer

    // Apply properties to the block component
    void Awake()
    {
        if (TryGetComponent<Block>(out var block))
        {
            block.weight = weight;
            block.maxStressCapacity = maxStressCapacity;
            block.lateralConnectionStrength = lateralConnectionStrength;
            block.diagonalConnectionStrength = diagonalConnectionStrength;
        }
    }
}
