using UnityEngine;
using UnityEngine.Pool;

public class PlatformGenerator : GeneratorBase
{
    [SerializeField] private GameObject _normalPlatformPrefab;
    [SerializeField] private GameObject _movingPlatformPrefab;
    [SerializeField] private GameObject _disappearingPlatformPrefab;
    [SerializeField] private GameObject _player;

    private IObjectPool<GameObject> _normalPlatformPool;
    private IObjectPool<GameObject> _movingPlatformPool;
    private IObjectPool<GameObject> _disappearingPlatformPool;

    protected override void Awake()
    {
        base.Awake();

        Transform platformGroup = new GameObject("Platforms").transform;
        _normalPlatformPool = CreatePool(_normalPlatformPrefab, platformGroup);
        _movingPlatformPool = CreatePool(_movingPlatformPrefab, platformGroup);
        _disappearingPlatformPool = CreatePool(_disappearingPlatformPrefab, platformGroup);
    }

    public override void Generate(float height)
    {
        IObjectPool<GameObject> pool;
        float random = Random.Range(0f, 1f);

        if (random < Settings.NormalPlatformFrequency)
            pool = _normalPlatformPool;
        else if (random < Settings.NormalPlatformFrequency + Settings.MovingPlatformFrequency && height > Settings.MovingPlatformMinHeight)
            pool = _movingPlatformPool;
        else if (height > Settings.DisappearingPlatformMinHeight)
            pool = _disappearingPlatformPool;
        else
            pool = _normalPlatformPool;

        GameObject platform = pool.Get();
        ActiveObjects[platform] = pool;
        platform.transform.position = GetRandomPosition(height);
    }

    public Vector2 SpawnNormalPlatform(float height)
    {
        GameObject platform = _normalPlatformPool.Get();
        Vector2 position = new(GetRandomPositionX(), height);
        platform.transform.position = position;
        ActiveObjects[platform] = _normalPlatformPool;

        return position;
    }

    public void PlacePlayerAboveFirstPlatform(Vector2 platformPosition) =>
        _player.transform.position = platformPosition + Vector2.up * 0.6f;

    public void ReleasePlatform(DisappearingPlatform disappearingPlatform)
    {
        GameObject platformObject = disappearingPlatform.gameObject;
        _disappearingPlatformPool.Release(platformObject);
        ActiveObjects.Remove(platformObject);
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
