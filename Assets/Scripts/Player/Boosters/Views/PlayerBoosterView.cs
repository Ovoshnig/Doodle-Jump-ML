using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public abstract class PlayerBoosterView : MonoBehaviour
{
    private const string IsRunningName = "isRunning";

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    protected Animator Animator => _animator;

    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    public void Enable(float duration)
    {
        _spriteRenderer.enabled = true;
        RunAnimation(duration);
    }

    public void Disable()
    {
        _spriteRenderer.enabled = false;
        StopAnimation();
    }

    public void Flip(bool flip)
    {
        Vector2 position = transform.localPosition;
        position.x = -position.x;
        transform.localPosition = position;
        _spriteRenderer.flipX = flip;
    }

    protected virtual void RunAnimation(float targetDuration) => _animator.SetBool(IsRunningName, true);

    protected virtual void StopAnimation() => _animator.SetBool(IsRunningName, false);
}
