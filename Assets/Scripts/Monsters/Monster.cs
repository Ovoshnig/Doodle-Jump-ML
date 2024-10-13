using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public abstract class Monster : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private AudioSource _audioSource;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    protected virtual void OnEnable()
    {
        _rigidbody.bodyType = RigidbodyType2D.Static;
        _audioSource.Play();
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.contacts[0];
        Vector2 collisionNormal = contact.normal;
        
        if (collision.collider.TryGetComponent<PlayerMover>(out _))
        {
            if (collisionNormal.y < -0.5f)
            {
                _rigidbody.bodyType = RigidbodyType2D.Dynamic;
                _audioSource.Stop();
            }
        }
    }
}
