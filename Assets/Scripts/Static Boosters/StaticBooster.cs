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
        if (collision.collider.TryGetComponent<PlayerLegs>(out _) 
            && collision.contacts[0].normal.y < 0f)
        {
            _spriteRenderer.sprite = _expandedSpringSprite;
            _audioSource.Play();
            _collider.enabled = false;
        }
    }
}
