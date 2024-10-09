using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    private const string HorizontalAxisName = "Horizontal";

    [SerializeField, Min(0f)] private float _platformJumpForce = 1f;
    [SerializeField, Min(0f)] private float _monsterJumpForce = 1f;
    [SerializeField, Min(0f)] private float _horizontalSpeed = 1f;
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

    public event Action PlatformJumpedOff;
    public event Action<float> NewHeightReached;
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

        _spriteRenderer.sprite = _rigidbody.linearVelocityY > 5f ? _legsTuckedSprite : _normalSprite;
    }

    private void FixedUpdate() => _rigidbody.linearVelocityX = _horizontalInput * _horizontalSpeed;

    private void OnBecameInvisible() => Lose();

    private void OnBodyCollided(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<Monster>(out _))
        {
            CrashedIntoMonster?.Invoke();
            Lose();
        }
        else if (collision.collider.TryGetComponent<Hole>(out _))
        {
            FlewIntoHole?.Invoke();
            Lose();
        }
    }

    private void OnLegsCollided(Collision2D collision)
    {
        if (_rigidbody.linearVelocityY <= 0f)
        {
            if (collision.collider.TryGetComponent(out Platform platform))
            {
                Jump(platform.transform, _platformJumpForce);
                PlatformJumpedOff?.Invoke();
            }
            else if (collision.collider.TryGetComponent(out Monster monster))
            {
                Jump(monster.transform, _monsterJumpForce);
                MonsterDowned?.Invoke();
            }
        }

        if (collision.collider.TryGetComponent<Hole>(out _))
        {
            FlewIntoHole?.Invoke();
            Lose();
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

        while (playerBooster.IsRunning)
        {
            NewHeightReached(transform.position.y);

            yield return null;
        }

        playerBoosterView.StopRunningAnimation();
        Jump(transform, _platformJumpForce);
    }
}
