using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class PlatformGenerator : GeneratorBase
{
    [SerializeField] private GameObject _normalPlatformPrefab;
    [SerializeField] private GameObject _movingPlatformPrefab;
    [SerializeField] private GameObject _disappearingPlatformPrefab;
    [SerializeField] private GameObject _player;

    private IObjectPool<GameObject> _normalPlatformPool;
    private IObjectPool<GameObject> _movingPlatformPool;
    private IObjectPool<GameObject> _disappearingPlatformPool;

    private readonly List<GameObject> _activePlatforms = new();

    private void Awake()
    {
        Transform platformGroup = new GameObject("Platforms").transform;
        _normalPlatformPool = CreatePool(_normalPlatformPrefab, platformGroup);
        _movingPlatformPool = CreatePool(_movingPlatformPrefab, platformGroup);
        _disappearingPlatformPool = CreatePool(_disappearingPlatformPrefab, platformGroup);
    }

    public override void Generate(float height)
    {
        GameObject platformPrefab;
        float random = Random.Range(0f, 1f);

        if (random < Settings.NormalPlatformFrequency)
            platformPrefab = _normalPlatformPool.Get();
        else if (random < Settings.NormalPlatformFrequency + Settings.MovingPlatformFrequency && height > Settings.MovingPlatformMinHeight)
            platformPrefab = _movingPlatformPool.Get();
        else if (height > Settings.DisappearingPlatformMinHeight)
            platformPrefab = _disappearingPlatformPool.Get();
        else
            platformPrefab = _normalPlatformPool.Get();

        platformPrefab.transform.position = GetRandomPosition(height);
        _activePlatforms.Add(platformPrefab);
    }

    public Vector2 SpawnNormalPlatform(float height)
    {
        GameObject platform = _normalPlatformPool.Get();
        Vector2 position = new(GetRandomPositionX(), height);
        platform.transform.position = position;
        _activePlatforms.Add(platform);

        return position;
    }

    public void PlacePlayerAboveFirstPlatform(Vector2 platformPosition) =>
        _player.transform.position = platformPosition + Vector2.up * 0.6f;

    public override void RemoveOffScreenElements(float cameraHeight)
    {
        for (int i = _activePlatforms.Count - 1; i >= 0; i--)
        {
            if (_activePlatforms[i].transform.position.y < cameraHeight - Camera.main.orthographicSize)
            {
                if (_activePlatforms[i].TryGetComponent<NormalPlatform>(out _))
                    _normalPlatformPool.Release(_activePlatforms[i]);
                else if (_activePlatforms[i].TryGetComponent<MovingPlatform>(out _))
                    _movingPlatformPool.Release(_activePlatforms[i]);
                else if (_activePlatforms[i].TryGetComponent<DisappearingPlatform>(out _))
                    _disappearingPlatformPool.Release(_activePlatforms[i]);

                _activePlatforms.RemoveAt(i);
            }
        }
    }

    public void ReleasePlatform(DisappearingPlatform disappearingPlatform)
    {
        GameObject platformObject = disappearingPlatform.gameObject;
        _disappearingPlatformPool.Release(platformObject);
        _activePlatforms.Remove(platformObject);
    }

    protected override Vector2 GetRandomPosition(float height) => new(GetRandomPositionX(), height);

    private float GetRandomPositionX() => Random.Range(-2.3f, 2.3f);

    private ObjectPool<GameObject> CreatePool(GameObject prefab, Transform groupTransform)
    {
        return new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject platform = Instantiate(prefab, groupTransform);
                platform.GetComponent<Platform>().SetPlatformGenerator(this);

                return platform;
            },
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj));
    }
}
