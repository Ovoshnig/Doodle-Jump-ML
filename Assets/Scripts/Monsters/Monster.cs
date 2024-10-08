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

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<PlayerLegs>(out _))
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _audioSource.Stop();
        }
    }

    protected virtual void Update()
    {
    }
}
