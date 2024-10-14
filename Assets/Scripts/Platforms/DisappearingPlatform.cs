using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class DisappearingPlatform : Platform
{
    [SerializeField] private AudioClip _disableClip;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private AudioSource _audioSource;

    protected override void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _spriteRenderer.enabled = true;
        _collider.enabled = true;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.contacts[0];
        Vector2 collisionNormal = contact.normal;

        if (collision.collider.TryGetComponent<PlayerMover>(out _))
        {
            if (collisionNormal.y < -0.5f)
            {
                _audioSource.PlayOneShot(_disableClip);
                StartCoroutine(DisableAfterSound());
            }
        }
    }

    private System.Collections.IEnumerator DisableAfterSound()
    {
        _spriteRenderer.enabled = false;
        _collider.enabled = false;

        yield return new WaitForSeconds(_disableClip.length);

        gameObject.SetActive(false);
    }
}
