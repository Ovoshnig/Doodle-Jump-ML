using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    private const string HorizontalAxisName = "Horizontal";

    [SerializeField, Min(0f)] private float _jumpForce = 1f;
    [SerializeField, Min(0f)] private float _horizontalSpeed = 1f;
    [SerializeField, Min(0f)] private float _maxVelocityMagnitude = 1f;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private Collider2D _bodyCollider;
    private Collider2D _legsCollider;
    private PlayerBody _playerBody;
    private PlayerLegs _playerLegs;
    private PlayerPropeller _playerPropeller;
    private PlayerJetpack _playerJetpack;
    private float _horizontalInput = 0f;
    private float _maxReachedHeight = 0f;

    public event Action PlatformJumpedOff;
    public event Action<float> NewHeightReached;
    public event Action Lost;
    public event Action MonsterDowned;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _bodyCollider = GetComponentInChildren<CapsuleCollider2D>();
        _legsCollider = GetComponentInChildren<BoxCollider2D>();
        _playerBody = GetComponentInChildren<PlayerBody>();
        _playerLegs = GetComponentInChildren<PlayerLegs>();
        _playerPropeller = GetComponentInChildren<PlayerPropeller>();
        _playerJetpack = GetComponentInChildren<PlayerJetpack>();

        _playerBody.Collided += OnBodyCollided;
        _playerBody.Triggered += OnBodyTriggered;
        _playerLegs.Collided += OnLegsCollided;
        _playerLegs.Triggered += OnLegsTriggered;
    }

    private void Start() => NewHeightReached?.Invoke(_maxReachedHeight);

    private void OnDestroy()
    {
        _playerBody.Collided -= OnBodyCollided;
        _playerBody.Triggered -= OnBodyTriggered;
        _playerLegs.Collided -= OnLegsCollided;
        _playerLegs.Triggered -= OnLegsTriggered;
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxis(HorizontalAxisName);

        if (_horizontalInput > 0f && !_spriteRenderer.flipX)
        {
            _spriteRenderer.flipX = true;
            _playerPropeller.Flip(true);
            _playerJetpack.Flip(true);
        }
        else if (_horizontalInput < 0f && _spriteRenderer.flipX)
        {
            _spriteRenderer.flipX = false;
            _playerPropeller.Flip(false);
            _playerJetpack.Flip(false);
        }

        Vector2 position = transform.position;

        if (Mathf.Abs(position.x) > 2.7f)
        {
            position.x = -Mathf.Sign(position.x) * 2.69f;
            transform.position = position;
        }
    }

    private void FixedUpdate()
    {
        if (_horizontalInput == 0)
            _rigidbody.linearVelocityX /= 1.2f;
        else
            _rigidbody.AddForceX(_horizontalSpeed * _horizontalInput, ForceMode2D.Impulse);

        _rigidbody.linearVelocityX = Vector2.ClampMagnitude(_rigidbody.linearVelocity, _maxVelocityMagnitude).x;
    }

    private void OnBecameInvisible() => Lose();

    private void OnBodyCollided(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<Monster>(out _))
            Lose();
        else if (collision.collider.TryGetComponent<Hole>(out _))
            Lose();
    }

    private void OnLegsCollided(Collision2D collision)
    {
        if (_rigidbody.linearVelocityY <= 0f)
        {
            if (collision.collider.TryGetComponent(out Platform platform))
            {
                Jump(platform.transform);
                PlatformJumpedOff?.Invoke();
            }
            else if (collision.collider.TryGetComponent(out Monster monster))
            {
                Jump(monster.transform);
                MonsterDowned?.Invoke();
            }
        }

        if (collision.collider.TryGetComponent<Hole>(out _))
            Lose();
    }

    private void OnBodyTriggered(Collider2D collider)
    {
        if (collider.TryGetComponent(out Booster booster))
            Boost(booster);
    }

    private void OnLegsTriggered(Collider2D collider)
    {
        if (collider.TryGetComponent(out Booster booster))
            Boost(booster);
    }

    private void Jump(Transform collisionTransform)
    {
        _rigidbody.AddForceY(_jumpForce, ForceMode2D.Impulse);
        float collisionPositionY = collisionTransform.position.y;

        if (collisionPositionY > _maxReachedHeight)
        {
            _maxReachedHeight = collisionPositionY;
            NewHeightReached?.Invoke(_maxReachedHeight);
        }
    }

    private void Boost(Booster booster)
    {
        if (booster is Propeller)
        {
            _playerPropeller.Enable();
        }
        else if (booster is Jetpack)
        {
            _playerJetpack.Enable();
        }
    }

    private void Lose()
    {
        _bodyCollider.enabled = false;
        _legsCollider.enabled = false;
        _rigidbody.linearVelocityY = 0f;
        Lost?.Invoke();
    }
}
