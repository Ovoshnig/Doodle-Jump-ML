using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public abstract class StaticBooster : MonoBehaviour
{
    [SerializeField] private Sprite _compressedSpringSprite;
    [SerializeField] private Sprite _expandedSpringSprite;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private AudioSource _audioSource;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _spriteRenderer.sprite = _compressedSpringSprite;
        _collider.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.contacts[0];
        Vector2 collisionNormal = contact.normal;

        if (collision.collider.TryGetComponent<PlayerMover>(out _))
        {
            if (collisionNormal.y < -0.5f)
            {
                _spriteRenderer.sprite = _expandedSpringSprite;
                _audioSource.Play();
                _collider.enabled = false;
            }
        }
    }
}
