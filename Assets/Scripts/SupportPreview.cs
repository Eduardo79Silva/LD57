using UnityEngine;

public class SupportPreview : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public void SetPosition(GridPosition pos)
    {
        transform.position = pos.ToWorld();
    }

    public void SetValid(bool isValid)
    {
        spriteRenderer.color = isValid ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
    }
}
