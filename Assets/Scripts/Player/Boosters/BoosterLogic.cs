using System;
using System.Collections;
using UnityEngine;

public class BoosterLogic : MonoBehaviour
{
    [SerializeField, Min(1f)] private float _distance = 50f;
    [SerializeField, Min(0f)] private float _duration = 3f;
    [SerializeField, Min(0f)] private float _decelerationDuration = 0.5f;
    [SerializeField] private Transform _playerTransform;

    private Rigidbody2D _playerRigidbody;

    public event Action Launched;
    public event Action Stopped;

    public bool IsWorking { get; private set; } = false;

    private void Awake() => _playerRigidbody = _playerTransform.GetComponent<Rigidbody2D>();

    public void Launch() => StartCoroutine(BoostRoutine());

    private IEnumerator BoostRoutine()
    {
        float speed = _distance / _duration;
        float startPositionY = _playerTransform.position.y;
        float endPositionY = startPositionY + _distance;
        IsWorking = true;
        Launched?.Invoke();

        while (_playerTransform.position.y < endPositionY)
        {
            _playerRigidbody.linearVelocityY = speed;
            yield return null;
        }

        Stopped?.Invoke();

        float decelerationTime = 0f;

        while (decelerationTime < _decelerationDuration)
        {
            decelerationTime += Time.deltaTime;
            float t = decelerationTime / _decelerationDuration;
            _playerRigidbody.linearVelocityY = Mathf.Lerp(speed, 0f, t);

            yield return null;
        }

        _playerRigidbody.linearVelocityY = 0f;
        IsWorking = false;
    }
}
