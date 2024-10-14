using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class BreakablePlatform : Platform
{
    private const string IsBreakingName = "isBreaking";

    private Collider2D _collider;
    private AudioSource _audioSource;
    private Animator _animator;

    protected override void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable() => _collider.enabled = true;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 position = transform.position;
        Vector2 colliderPosition = collision.transform.position;
        Vector2 direction = (position - colliderPosition).normalized;

        if (collision.TryGetComponent<PlayerMover>(out _))
        {
            if (direction.y < -0.5f)
            {
                _collider.enabled = false;
                _audioSource.Play();
                _animator.SetBool(IsBreakingName, true);
            }
        }
    }

    private void Disable()
    {
        _animator.SetBool(IsBreakingName, false);
        gameObject.SetActive(false);
    }
}
