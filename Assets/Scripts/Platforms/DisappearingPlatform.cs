using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
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
        if (collision.collider.TryGetComponent<PlayerMover>(out _))
        {
            if (collision.gameObject.TryGetComponent(out Rigidbody2D rigidbody)
                && rigidbody.linearVelocityY <= 0f)
            {
                _audioSource.PlayOneShot(_disableClip);
                StartCoroutine(ReleaseAfterSound());
            }
        }
    }

    private System.Collections.IEnumerator ReleaseAfterSound()
    {
        _spriteRenderer.enabled = false;
        _collider.enabled = false;

        yield return new WaitForSeconds(_disableClip.length);

        PlatformGenerator.ReleaseDisappearingPlatform(this);
    }
}
