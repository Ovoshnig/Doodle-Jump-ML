using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    private const string HorizontalAxisName = "Horizontal";

    [SerializeField, Min(0f)] private float _platformJumpForce = 7.4f;
    [SerializeField, Min(0f)] private float _monsterJumpForce = 8.2f;
    [SerializeField, Min(0f)] private float _springJumpForce = 20f;
    [SerializeField, Min(0f)] private float _horizontalSpeed = 4.8f;
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _legsTuckedSprite;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private Collider2D _bodyCollider;
    private Collider2D _legsCollider;
    private PlayerBody _playerBody;
    private PlayerLegs _playerLegs;
    private PlayerPropeller _playerPropeller;
    private PlayerJetpack _playerJetpack;
    private PlayerPropellerView _playerPropellerView;
    private PlayerJetpackView _playerJetpackView;
    private Camera _camera;
    private float _horizontalInput = 0f;
    private float _maxReachedHeight = 0f;
    private bool _isLost = false;

    public event Action PlatformJumpedOff;
    public event Action<float, bool> NewHeightReached;
    public event Action Lost;
    public event Action CrashedIntoMonster;
    public event Action FlewIntoHole;
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
        _playerPropellerView = GetComponentInChildren<PlayerPropellerView>();
        _playerJetpackView = GetComponentInChildren<PlayerJetpackView>();
        _camera = Camera.main;

        _playerBody.Collided += OnBodyCollided;
        _playerBody.Triggered += OnBodyTriggered;
        _playerLegs.Collided += OnLegsCollided;
        _playerLegs.Triggered += OnLegsTriggered;
    }

    private void Start() => NewHeightReached?.Invoke(_maxReachedHeight, false);

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
        Vector2 position = transform.position;
        Vector3 scale = transform.localScale;
        Vector3 viewportPosition = _camera.WorldToViewportPoint(position);

        if (_horizontalInput > 0f && scale.x > 0f)
            scale.x = -Mathf.Abs(scale.x);
        else if (_horizontalInput < 0f && scale.x < 0f)
            scale.x = Mathf.Abs(scale.x);

        transform.localScale = scale;

        if (viewportPosition.x < 0f || viewportPosition.x > 1f)
        {
            viewportPosition.x = 1f - viewportPosition.x;
            position = _camera.ViewportToWorldPoint(viewportPosition);
            position.x -= Mathf.Sign(viewportPosition.x) * 0.1f;
            transform.position = position;
        }

        if (viewportPosition.y < 0f)
            Lose();

        _spriteRenderer.sprite = _rigidbody.linearVelocityY > 5f ? _legsTuckedSprite : _normalSprite;
    }

    private void FixedUpdate() => _rigidbody.linearVelocityX = _horizontalInput * _horizontalSpeed;

    private void OnBodyCollided(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<MonsterBody>(out _))
        {
            CrashedIntoMonster?.Invoke();
            _rigidbody.linearVelocityY = 0f;
            DisableColliders();
        }
        else if (collision.collider.TryGetComponent<Hole>(out _))
        {
            FlewIntoHole?.Invoke();
            _rigidbody.linearVelocityY = 0f;
            DisableColliders();
        }
    }

    private void OnLegsCollided(Collision2D collision)
    {
        Debug.Log(collision.collider.name);
        Debug.Log(collision.contacts[0].normal.y);

        if (collision.contacts[0].normal.y is > 0.9f and <= 1f)
        {
            if (collision.collider.TryGetComponent(out Platform platform))
            {
                Jump(platform.transform, _platformJumpForce);
                PlatformJumpedOff?.Invoke();
            }
            else if (collision.collider.TryGetComponent(out MonsterHead monsterHead))
            {
                Jump(monsterHead.transform, _monsterJumpForce);
                MonsterDowned?.Invoke();
            }
            else if (collision.collider.TryGetComponent(out StaticBooster staticBooster))
            {
                StaticBoost(staticBooster);
            }
        }

        if (collision.collider.TryGetComponent<Hole>(out _))
        {
            FlewIntoHole?.Invoke();
            _rigidbody.linearVelocityY = 0f;
            DisableColliders();
        }
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

    private void Jump(Transform collisionTransform, float force = 1f)
    {
        _rigidbody.AddForceY(force, ForceMode2D.Impulse);
        float collisionPositionY = collisionTransform.position.y;

        if (collisionPositionY > _maxReachedHeight)
        {
            _maxReachedHeight = collisionPositionY;
            NewHeightReached?.Invoke(_maxReachedHeight, false);
        }
    }

    private void Lose()
    {
        if (_isLost)
            return;

        DisableColliders();
        _isLost = true;
        Lost?.Invoke();
    }

    private void Boost(Booster booster)
    {
        if (booster is Propeller)
            StartCoroutine(BoostRoutine(_playerPropeller, _playerPropellerView));
        else if (booster is Jetpack)
            StartCoroutine(BoostRoutine(_playerJetpack, _playerJetpackView));
    }

    private IEnumerator BoostRoutine(PlayerBooster playerBooster, PlayerBoosterView playerBoosterView)
    {
        float duration = playerBooster.Run();
        playerBoosterView.Enable(duration);
        DisableColliders();

        while (playerBooster.IsRunning)
        {
            NewHeightReached(transform.position.y, true);

            yield return null;
        }

        playerBoosterView.StopRunningAnimation();
        Jump(transform, _platformJumpForce);
        EnableColliders();
    }

    private void StaticBoost(StaticBooster staticBooster)
    {
        if (staticBooster is Spring)
            StartCoroutine(StaticBoostRoutine(_springJumpForce));
    }

    private IEnumerator StaticBoostRoutine(float force)
    {
        _rigidbody.AddForceY(force, ForceMode2D.Impulse);

        while (_rigidbody.linearVelocityY >= 0)
        {
            float positionY = transform.position.y;
            _maxReachedHeight = positionY;
            NewHeightReached(positionY, true);

            yield return null;
        }
    }

    private void EnableColliders()
    {
        _bodyCollider.enabled = true;
        _legsCollider.enabled = true;
    }

    private void DisableColliders()
    {
        _bodyCollider.enabled = false;
        _legsCollider.enabled = false;
    }
}
