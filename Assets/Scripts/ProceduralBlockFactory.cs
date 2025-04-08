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

    // Define these as fields in your class
    private float noiseOffsetX;
    private float noiseOffsetY;

    public void Initialization(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }

    void Start()
    {
        // Initialize once, so the noise pattern remains consistent across calls
        noiseOffsetX = Random.Range(0f, 1000f);
        noiseOffsetY = Random.Range(0f, 1000f);
    }

    private Block FindNeighborOre(int x, int y)
    {
        // Collect all neighbors that have an ore (not default).
        List<Block> foundOres = new();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;
                int nx = x + dx;
                int ny = y + dy;
                if (!gridManager.IsInBounds(nx, ny))
                    continue;

                Block block = gridManager.GetBlockAt(nx, ny);
                if (block != null && block.oreType != defaultPrefab.GetComponent<Block>().oreType)
                {
                    foundOres.Add(block);
                }
            }
        }

        // If no ore neighbors, return null.
        if (foundOres.Count == 0)
            return null;

        // do a "majority vote" among foundOres if you want clusters to
        // favor the most common neighbor ore type.
        Dictionary<OreType, int> oreCount = new();
        foreach (var ore in foundOres)
        {
            if (!oreCount.ContainsKey(ore.oreType))
            {
                oreCount[ore.oreType] = 0;
            }
            oreCount[ore.oreType]++;
        }
        OreType mostCommonOre = defaultPrefab.GetComponent<Block>().oreType;

        int maxCount = 0;
        foreach (var kvp in oreCount)
        {
            if (kvp.Value > maxCount)
            {
                maxCount = kvp.Value;
                mostCommonOre = kvp.Key;
            }
        }
        // Return the first block with the most common ore type.
        foreach (var ore in foundOres)
        {
            if (ore.oreType == mostCommonOre)
            {
                return ore;
            }
        }

        // This should never happen if the logic is correct.
        throw new System.Exception("No ore found in neighbors!");
    }

    /// <summary>
    /// Selects a block prefab based on grid position, local depth ratio, and ore noise.
    /// </summary>
    public GameObject GetBlockPrefab(int x, int y, float depthRatio, float oreNoise)
    {
        List<(GameObject prefab, float weight)> candidates = new();

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

        // If no candidate is available, return default.
        if (candidates.Count == 0)
            return defaultPrefab;

        Block surroundingOre = FindNeighborOre(x, y);
        if (surroundingOre != null)
        {
            // Filter candidates to that ore type only.
            List<(GameObject prefab, float weight)> newCandidates = candidates.FindAll(candidate =>
            {
                Block blockComp = candidate.prefab.GetComponent<Block>();
                return blockComp != null && blockComp.oreType == surroundingOre.oreType;
            });

            if (newCandidates.Count > 0)
                candidates = newCandidates;
        }

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
        // Apply the random offsets to the noise coordinates
        float oreNoise = Mathf.PerlinNoise(
            (x + noiseOffsetX) * oreNoiseScale,
            (y + noiseOffsetY) * oreNoiseScale
        );
        GameObject chosenPrefab = GetBlockPrefab(x, y, depthRatio, oreNoise);
        GameObject blockInstance = Instantiate(chosenPrefab, spawnPos, Quaternion.identity);

        // Adjust block scale so it fills the grid cell.
        blockInstance.transform.localScale = Vector3.one * gridManager.cellSize;

        return blockInstance;
    }
}
