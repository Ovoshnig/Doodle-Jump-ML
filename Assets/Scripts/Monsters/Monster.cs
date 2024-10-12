using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public abstract class Monster : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private AudioSource _audioSource;
    private MonsterBody _monsterBody;
    private MonsterHead _monsterHead;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _monsterBody = GetComponentInChildren<MonsterBody>();
        _monsterHead = GetComponentInChildren<MonsterHead>();
    }

    protected virtual void OnEnable()
    {
        _rigidbody.bodyType = RigidbodyType2D.Static;
        _audioSource.Play();

        _monsterBody.Collided += OnBodyCollided;
        _monsterHead.Collided += OnHeadCollided;
    }

    protected virtual void Start()
    {
    }

    private void OnDisable()
    {
        _monsterBody.Collided -= OnBodyCollided;
        _monsterHead.Collided -= OnHeadCollided;
    }

    protected virtual void Update()
    {
    }

    private void OnBodyCollided(Collision2D collision)
    {
        
    }

    private void OnHeadCollided(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<PlayerLegs>(out _))
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _audioSource.Stop();
        }
    }
}
