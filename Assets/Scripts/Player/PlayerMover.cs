using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    private const string HorizontalAxisName = "Horizontal";

    [SerializeField, Min(0f)] private float _jumpForce = 1f;
    [SerializeField, Min(0f)] private float _horizontalSpeed = 1f;
    [SerializeField, Min(0f)] private float _maxVelocityMagnitude = 1f;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Rigidbody2D _rigidbody;
    private PlayerBody _playerBody;
    private Camera _mainCamera;
    private Transform _cameraTransform;
    private float _horizontalInput = 0f;
    private float _maxReachedHeight = 0f;
    private float _screenBoundPositionY;

    public event Action PlatformJumpedOff;
    public event Action<float> NewHeightReached;
    public event Action Lost;
    public event Action MonsterDowned;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerBody = GetComponentInChildren<PlayerBody>();

        _playerBody.WithMonsterCollided += OnWithMonsterCollided;

        _mainCamera = Camera.main;
        _cameraTransform = _mainCamera.transform;
    }

    private void Start()
    {
        NewHeightReached?.Invoke(_maxReachedHeight);
        _screenBoundPositionY = _mainCamera.orthographicSize;
    }

    private void OnDestroy() => _playerBody.WithMonsterCollided -= OnWithMonsterCollided;

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

        if (position.y < _cameraTransform.position.y - _screenBoundPositionY)
            Lose();
    }

    private void FixedUpdate()
    {
        if (_horizontalInput == 0)
            _rigidbody.linearVelocityX /= 1.2f;
        else
            _rigidbody.AddForceX(_horizontalSpeed * _horizontalInput, ForceMode2D.Impulse);

        _rigidbody.linearVelocityX = Vector2.ClampMagnitude(_rigidbody.linearVelocity, _maxVelocityMagnitude).x;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_rigidbody.linearVelocityY <= 0f)
        {
            if (collision.gameObject.TryGetComponent<Platform>(out _))
            {
                Jump(collision.transform);
                PlatformJumpedOff?.Invoke();
            }
            else if (collision.gameObject.TryGetComponent<Monster>(out _))
            {
                Jump(collision.transform);
                MonsterDowned?.Invoke();
            }
        }
    }

    private void OnWithMonsterCollided() => Lose();

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
        _collider.enabled = false;
        Lost?.Invoke();
    }
}
