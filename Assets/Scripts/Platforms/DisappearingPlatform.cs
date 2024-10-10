using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class DisappearingPlatform : Platform
{
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private AudioSource _audioSource;

    protected override void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<PlayerLegs>(out _))
        {
            if (collision.gameObject.TryGetComponent(out Rigidbody2D rigidbody)
                && rigidbody.linearVelocityY <= 0f)
            {
                _audioSource.Play();
                _spriteRenderer.enabled = false;
                _collider.enabled = false;
            }
        }
    }
}
