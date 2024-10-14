using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Camera))]
public class CameraMover : MonoBehaviour
{
    [SerializeField] private PlayerMover _playerMover;
    [SerializeField] private float _cameraVerticalOffset = 2.5f;
    [SerializeField] private float _cameraMoveSpeed = 2.5f;

    private Coroutine _cameraCoroutine;

    private void OnEnable()
    {
        _playerMover.EpisodeBegan += OnEpisodeBegan;
        _playerMover.NewHeightReached += OnNewHeightReached;
    }


    private void OnDisable()
    {
        _playerMover.EpisodeBegan -= OnEpisodeBegan;
        _playerMover.NewHeightReached -= OnNewHeightReached;
    }

    private void OnEpisodeBegan()
    {
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
    }

    private void OnNewHeightReached(float newHeight, bool usingBooster)
    {
        Vector3 position = transform.position;
        Vector3 targetPosition = transform.position;
        targetPosition.y = newHeight;

        if (_cameraCoroutine != null)
            StopCoroutine(_cameraCoroutine);

        if (usingBooster && position.y < targetPosition.y)
        {
            transform.position = targetPosition;
        }
        else
        {
            targetPosition.y += _cameraVerticalOffset;
            _cameraCoroutine = StartCoroutine(MoveCameraSmoothly(targetPosition));
        }
    }

    private IEnumerator MoveCameraSmoothly(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, _cameraMoveSpeed * Time.deltaTime);

            yield return null;
        }

        transform.position = targetPosition;
    }
}
