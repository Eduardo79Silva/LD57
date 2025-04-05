using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockProbability
{
    public GameObject prefab; // The block prefab (e.g. dirt, iron, gold, diamond)
    public float weight = 1f; // Base probability weight.
    public float minDepth = 0f; // Valid depth ratio minimum (0 = surface)
    public float maxDepth = 1f; // Valid depth ratio maximum (1 = deepest)
}

public class ProceduralBlockFactory : BlockFactory
{
    public GameObject defaultPrefab;
    public BlockProbability[] blockProbabilities;

    private GridManager gridManager;

    public void Initialization(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }

    /// <summary>
    /// Selects a block prefab based on grid position, local depth ratio, and ore noise.
    /// </summary>
    public GameObject GetBlockPrefab(int x, int y, float depthRatio, float oreNoise)
    {
        List<(GameObject prefab, float weight)> candidates = new List<(GameObject, float)>();

        // Define a threshold for ore noise.
        float oreNoiseThreshold = 0.65f;

        foreach (var bp in blockProbabilities)
        {
            if (depthRatio >= bp.minDepth && depthRatio <= bp.maxDepth)
            {
                float effectiveWeight = bp.weight;
                // If this block type is not the default (e.g. dirt), modulate weight using oreNoise.
                if (bp.prefab != defaultPrefab)
                {
                    if (oreNoise > oreNoiseThreshold)
                    {
                        float oreFactor = (oreNoise - oreNoiseThreshold) / (1f - oreNoiseThreshold);
                        effectiveWeight *= oreFactor;
                    }
                    else
                    {
                        effectiveWeight = 0f;
                    }
                }
                candidates.Add((bp.prefab, effectiveWeight));
            }
        }

        Debug.Log($"Candidates for ({x}, {y}): {candidates.Count}");

        // If no candidate is available, return default.
        if (candidates.Count == 0)
            return defaultPrefab;

        float totalWeight = 0f;
        foreach (var (prefab, weight) in candidates)
        {
            totalWeight += weight;
        }
        if (totalWeight <= 0f)
            return defaultPrefab;

        float randomValue = Random.Range(0f, totalWeight);
        foreach (var (prefab, weight) in candidates)
        {
            if (randomValue < weight)
            {
                return prefab;
            }
            randomValue -= weight;
        }
        return defaultPrefab;
    }

    public override GameObject GenerateBlock(int x, int y, float depthRatio, Vector3 spawnPos)
    {
        float oreNoiseScale = 0.2f;
        float oreNoise = Mathf.PerlinNoise(x * oreNoiseScale, y * oreNoiseScale);
        GameObject chosenPrefab = GetBlockPrefab(x, y, depthRatio, oreNoise);
        Debug.Log(
            $"Chosen prefab for ({x}, {y}) at depthRatio {depthRatio:F2}: {chosenPrefab.name}"
        );
        return Instantiate(chosenPrefab, spawnPos, Quaternion.identity);
    }
}
