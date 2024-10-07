using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public abstract class Monster : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private AudioSource _audioSource;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start() => _rigidbody.bodyType = RigidbodyType2D.Static;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerMover>(out _))
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _audioSource.loop = false;
            _audioSource.Stop();
        }
    }
}
