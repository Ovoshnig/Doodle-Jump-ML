using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class PlayerBooster : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    protected virtual void Awake() => _spriteRenderer = GetComponent<SpriteRenderer>();

    public void Enable() => _spriteRenderer.enabled = true;

    public void Disable() => _spriteRenderer.enabled = false;

    public void Flip(bool flip)
    {
        Vector2 position = transform.localPosition;
        position.x = -position.x;
        transform.localPosition = position;
        _spriteRenderer.flipX = flip;
    }
}
