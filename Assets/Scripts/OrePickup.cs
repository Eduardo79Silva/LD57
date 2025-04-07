using UnityEngine;

public class OrePickup : MonoBehaviour
{
    public Ore ore;
    public float magnetStrength = 5f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyMagnetForce(Vector2 direction)
    {
        rb.AddForce(direction * magnetStrength, ForceMode2D.Force);
    }
}
