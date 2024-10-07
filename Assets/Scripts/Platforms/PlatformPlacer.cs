using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class PlatformPlacer : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _minPlatformSpacing = 1f;
    [SerializeField, Min(0f)] private float _maxPlatformSpacing = 2f;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _normalPlatformPrefab;
    [SerializeField] private GameObject _movingPlatformPrefab;

    private ObjectPool<GameObject> _normalPlatformPool;
    private ObjectPool<GameObject> _movingPlatformPool;
    private PlayerMover _playerMover;
    private Camera _mainCamera;
    private float _screenBoundPositionX;
    private float _screenBoundPositionY;
    private float _maxPositionX;
    private float _currentPlatformHeight = 0f;

    private readonly List<GameObject> _activePlatforms = new();

    private void Awake()
    {
        _mainCamera = Camera.main;
        _playerMover = _player.GetComponent<PlayerMover>();
        _playerMover.NewPlatformReached += OnNewPlatformReached;
    }

    private void Start()
    {
        float cameraOrthographicSize = _mainCamera.orthographicSize;
        _screenBoundPositionY = cameraOrthographicSize;
        _screenBoundPositionX = cameraOrthographicSize * Screen.width / Screen.height;
        float platformSizeX = (_normalPlatformPrefab.GetComponent<BoxCollider2D>().size
            * _normalPlatformPrefab.transform.lossyScale).x / 2f;
        _maxPositionX = _screenBoundPositionX - platformSizeX;

        _normalPlatformPool = new ObjectPool<GameObject>(
            CreateNormalPlatform,
            OnGetFromPool,
            OnReturnToPool,
            OnDestroyPlatform);

        _movingPlatformPool = new ObjectPool<GameObject>(
            CreateMovingPlatform,
            OnGetFromPool,
            OnReturnToPool,
            OnDestroyPlatform);

        GenerateInitialPlatforms();
    }

    private void OnDestroy() => _playerMover.NewPlatformReached -= OnNewPlatformReached;

    private GameObject CreateNormalPlatform() => Instantiate(_normalPlatformPrefab);

    private GameObject CreateMovingPlatform() => Instantiate(_movingPlatformPrefab);

    private void OnGetFromPool(GameObject platform)
    {
        platform.SetActive(true);
        _activePlatforms.Add(platform);
    }

    private void OnReturnToPool(GameObject platform)
    {
        platform.SetActive(false);
        _activePlatforms.Remove(platform);
    }

    private void OnDestroyPlatform(GameObject platform) => Destroy(platform);

    private void GenerateInitialPlatforms()
    {
        Vector2 firstPlatformPosition = SpawnFirstPlatform();
        _player.transform.position = firstPlatformPosition + 0.6f * Vector2.up;

        while (_currentPlatformHeight < 2f * _screenBoundPositionY)
        {
            if (Random.Range(0, 6) == 0)
                SpawnMovingPlatform();
            else
                SpawnNormalPlatform();
        }
    }

    private Vector2 SpawnFirstPlatform()
    {
        GameObject platform = _normalPlatformPool.Get();
        platform.transform.position = new Vector2(GetRandomPositionX(), _currentPlatformHeight);
        _currentPlatformHeight += GetRandomPlatformSpacing();

        return platform.transform.position;
    }

    private void SpawnNormalPlatform()
    {
        GameObject platform = _normalPlatformPool.Get();
        platform.transform.position = new Vector2(GetRandomPositionX(), _currentPlatformHeight);
        _currentPlatformHeight += GetRandomPlatformSpacing();
    }

    private void SpawnMovingPlatform()
    {
        GameObject platform = _movingPlatformPool.Get();
        platform.transform.position = new Vector2(GetRandomPositionX(), _currentPlatformHeight);
        _currentPlatformHeight += GetRandomPlatformSpacing();
    }

    private void OnNewPlatformReached(float newHeight)
    {
        DisablePlatformsBelowCamera();
        SpawnPlatformsAboveCamera();
    }

    private void DisablePlatformsBelowCamera()
    {
        float cameraBottomY = _mainCamera.transform.position.y - _screenBoundPositionY;

        for (int i = _activePlatforms.Count - 1; i >= 0; i--)
            if (_activePlatforms[i].transform.position.y < cameraBottomY)
                _normalPlatformPool.Release(_activePlatforms[i]);
    }

    private void SpawnPlatformsAboveCamera()
    {
        float cameraTopY = _mainCamera.transform.position.y + _screenBoundPositionY;

        while (_currentPlatformHeight < cameraTopY + _screenBoundPositionY)
        {
            if (Random.Range(0, 6) == 0)
                SpawnMovingPlatform();
            else
                SpawnNormalPlatform();
        }
    }

    private float GetRandomPositionX() => Random.Range(-_maxPositionX, _maxPositionX);

    private float GetRandomPlatformSpacing() => Random.Range(_minPlatformSpacing, _maxPlatformSpacing);
}
