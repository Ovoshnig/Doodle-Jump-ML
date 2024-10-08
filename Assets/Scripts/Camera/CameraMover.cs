using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraMover : MonoBehaviour
{
    [SerializeField] private PlayerMover _playerMover;
    [SerializeField] private float _cameraVerticalOffset = 0f;
    [SerializeField] private float _cameraMoveSpeed = 2f;

    private Camera _mainCamera;
    private Coroutine _cameraCoroutine;

    private void Awake() => _mainCamera = transform.GetComponent<Camera>();

    private void OnEnable() => _playerMover.NewHeightReached += OnNewPlatformReached;

    private void OnDisable() => _playerMover.NewHeightReached -= OnNewPlatformReached;

    private void OnNewPlatformReached(float newHeight)
    {
        Vector3 targetPosition = transform.position;
        targetPosition.y = newHeight + _mainCamera.orthographicSize + _cameraVerticalOffset;

        if (_cameraCoroutine != null)
            StopCoroutine(_cameraCoroutine);

        _cameraCoroutine = StartCoroutine(MoveCameraSmoothly(targetPosition));
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
