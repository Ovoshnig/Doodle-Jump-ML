using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public abstract class PlayerBoosterView : MonoBehaviour
{
    private const string IsRunningName = "isRunning";

    [SerializeField] private AnimationClip _fallingClip;

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
        PlayRunningAnimation(duration);
    }

    public void Disable() => _spriteRenderer.enabled = false;

    public virtual void StopRunningAnimation()
    {
        _animator.SetBool(IsRunningName, false);
        StartCoroutine(WaitFallingAnimation());
    }

    protected virtual void PlayRunningAnimation(float targetDuration) => _animator.SetBool(IsRunningName, true);

    private IEnumerator WaitFallingAnimation()
    {
        yield return new WaitForSecondsRealtime(_fallingClip.length);

        Disable();
    }
}
