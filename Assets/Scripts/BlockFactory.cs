using UnityEngine;

public abstract class BlockFactory : MonoBehaviour
{
    // Updated signature to include depthRatio.
    public abstract GameObject GenerateBlock(int x, int y, float depthRatio, Vector3 spawnPos);
}
