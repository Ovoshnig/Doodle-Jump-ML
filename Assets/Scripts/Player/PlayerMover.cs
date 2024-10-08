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

        _playerBody.CollidedWithMonster += OnBodyCollidedWithMonster;
        _playerLegs.CollidedWithPlatform += OnLegsCollidedWithPlatform;
        _playerLegs.CollidedWithMonster += OnLegsCollidedWithMonster;
    }

    private void Start() => NewHeightReached?.Invoke(_maxReachedHeight);

    private void OnDestroy()
    {
        _playerBody.CollidedWithMonster -= OnBodyCollidedWithMonster;
        _playerLegs.CollidedWithPlatform -= OnLegsCollidedWithPlatform;
        _playerLegs.CollidedWithMonster -= OnLegsCollidedWithMonster;
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxis(HorizontalAxisName);

        if (_horizontalInput > 0f && !_spriteRenderer.flipX)
            _spriteRenderer.flipX = true;
        else if (_horizontalInput < 0f && _spriteRenderer.flipX)
            _spriteRenderer.flipX = false;

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

    private void OnBodyCollidedWithMonster() => Lose();

    private void OnLegsCollidedWithMonster(Transform monsterTransform)
    {
        if (_rigidbody.linearVelocityY <= 0f)
        {
            Jump(monsterTransform);
            MonsterDowned?.Invoke();
        }
    }

    private void OnLegsCollidedWithPlatform(Transform platformTransform)
    {
        if (_rigidbody.linearVelocityY <= 0f)
        {
            Jump(platformTransform);
            PlatformJumpedOff?.Invoke();
        }
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

    private void Lose()
    {
        _bodyCollider.enabled = false;
        _legsCollider.enabled = false;
        _rigidbody.linearVelocityY = 0f;
        Lost?.Invoke();
    }
}
