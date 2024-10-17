using UnityEngine;
using Unity.MLAgents;
using System;
using System.Collections;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerCollisionHandler))]
public class PlayerMover : Agent
{
    private const string HorizontalAxisName = "Horizontal";

    [SerializeField, Min(0f)] private float _platformJumpForce = 7.4f;
    [SerializeField, Min(0f)] private float _monsterJumpForce = 8.2f;
    [SerializeField, Min(0f)] private float _springJumpForce = 20f;
    [SerializeField, Min(0f)] private float _horizontalSpeed = 4.8f;
    [SerializeField, Min(0f)] private float _loseDuration = 0f;
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _legsTuckedSprite;
    [SerializeField] private BoosterLogic _propellerLogic;
    [SerializeField] private BoosterLogic _jetpackLogic;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Rigidbody2D _rigidbody;
    private PlayerCollisionHandler _collisionHandler;
    private Camera _camera;
    private float _maxReachedHeight = 0f;
    private bool _isLost = false;

    public event Action EpisodeBegan;
    public event Action<float, bool> NewHeightReached;
    public event Action Lost;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _collisionHandler = GetComponent<PlayerCollisionHandler>();

        _camera = Camera.main;

        _collisionHandler.PlatformJumpedOff += OnPlatformJumpedOff;
        _collisionHandler.MonsterDowned += OnMonsterDowned;
        _collisionHandler.CrashedIntoMonster += OnCrashedIntoMonster;
        _collisionHandler.FlewIntoHole += OnFlewIntoHole;
        _collisionHandler.BoosterUsed += OnBoosterUsed;
        _collisionHandler.StaticBoosterUsed += OnStaticBoosterUsed;
    }

    private void OnDestroy()
    {
        _collisionHandler.PlatformJumpedOff -= OnPlatformJumpedOff;
        _collisionHandler.MonsterDowned -= OnMonsterDowned;
        _collisionHandler.CrashedIntoMonster -= OnCrashedIntoMonster;
        _collisionHandler.FlewIntoHole -= OnFlewIntoHole;
        _collisionHandler.BoosterUsed -= OnBoosterUsed;
        _collisionHandler.StaticBoosterUsed -= OnStaticBoosterUsed;
    }

    private void Update()
    {
        Vector2 position = transform.position;
        Vector3 viewportPosition = _camera.WorldToViewportPoint(position);

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

    public override void OnEpisodeBegin()
    {
        _collider.enabled = true;
        _rigidbody.linearVelocity = Vector2.zero;
        _maxReachedHeight = 0f;
        _isLost = false;

        EpisodeBegan?.Invoke();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2 position = transform.position;

        sensor.AddObservation(position);
        sensor.AddObservation(_rigidbody.linearVelocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.DiscreteActions[0] - 1;
        _rigidbody.linearVelocityX = moveX * _horizontalSpeed;
        Vector3 scale = transform.localScale;

        if (moveX > 0f && scale.x > 0f)
            scale.x = -Mathf.Abs(scale.x);
        else if (moveX < 0f && scale.x < 0f)
            scale.x = Mathf.Abs(scale.x);

        transform.localScale = scale;

        AddReward(-0.002f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = (int)Input.GetAxisRaw(HorizontalAxisName) + 1;
    }

    private void OnPlatformJumpedOff(float height) => Jump(height, _platformJumpForce);

    private void OnMonsterDowned(float height) => Jump(height, _monsterJumpForce);

    private void OnCrashedIntoMonster()
    {
        _rigidbody.linearVelocityY = 0f;
        _collider.enabled = false;
        AddReward(-0.5f);
    }

    private void OnFlewIntoHole()
    {
        _rigidbody.linearVelocityY = 0f;
        _collider.enabled = false;
        AddReward(-0.5f);
    }

    private void OnBoosterUsed(Booster booster) => Boost(booster);

    private void OnStaticBoosterUsed(StaticBooster staticBooster) => StaticBoost(staticBooster);

    private void Jump(float height, float force = 1f)
    {
        _rigidbody.linearVelocityY = 0f;
        _rigidbody.AddForceY(force, ForceMode2D.Impulse);

        if (height > _maxReachedHeight)
        {
            AddReward(0.01f * (height - _maxReachedHeight));
            _maxReachedHeight = height;
            NewHeightReached?.Invoke(height, false);
        }
    }

    private void Lose()
    {
        if (_isLost)
            return;

        _collider.enabled = false;
        _isLost = true;
        AddReward(-1f);
        Lost?.Invoke();
        StartCoroutine(WaitLoseDuration());
    }

    private IEnumerator WaitLoseDuration()
    {
        yield return new WaitForSeconds(_loseDuration);

        EndEpisode();
    }

    private void Boost(Booster booster)
    {
        if (booster is Propeller)
            StartCoroutine(BoostRoutine(_propellerLogic));
        else if (booster is Jetpack)
            StartCoroutine(BoostRoutine(_jetpackLogic));
    }

    private IEnumerator BoostRoutine(BoosterLogic boosterLogic)
    {
        boosterLogic.Launch();
        _collider.enabled = false;

        while (boosterLogic.IsWorking)
        {
            float height = transform.position.y;
            AddReward(0.01f * (height - _maxReachedHeight));
            _maxReachedHeight = height;
            NewHeightReached(height, true);

            yield return null;
        }

        _collider.enabled = true;
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
            float height = transform.position.y;
            AddReward(0.01f * (height - _maxReachedHeight));
            _maxReachedHeight = height;
            NewHeightReached(height, true);

            yield return null;
        }
    }
}
