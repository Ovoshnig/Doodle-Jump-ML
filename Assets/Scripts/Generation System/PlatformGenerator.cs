using System.Collections.Generic;
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

        List<GameObject> objects = new()
        {
            _normalPlatformPrefab,
            _movingPlatformPrefab,
            _disappearingPlatformPrefab
        };

        float screenBoundX = Camera.ViewportToWorldPoint(new Vector2(1f, 0f)).x;

        foreach (var @object in objects)
        {
            BoxCollider2D collider = @object.GetComponent<BoxCollider2D>();
            Vector2 halfSize = 0.5f * collider.size * @object.transform.lossyScale;
            ObjectBoundsX[@object.name] = screenBoundX - halfSize.x;
            ObjectHalfSizesY[@object.name] = halfSize.y;
        }
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
        platform.transform.position = GetRandomPosition(platform, height);
    }

    public Vector2 SpawnNormalPlatform(float height)
    {
        GameObject platform = _normalPlatformPool.Get();
        Vector2 position = new(GetRandomPositionX(_normalPlatformPrefab), height);
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

    protected Vector2 GetRandomPosition(GameObject @object, float height) => new(GetRandomPositionX(@object), height);

    private float GetRandomPositionX(GameObject @object)
    {
        float boundX = ObjectBoundsX[@object.name];

        return Random.Range(-boundX, boundX);
    }

    private ObjectPool<GameObject> CreatePool(GameObject prefab, Transform groupTransform)
    {
        return new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject platform = Instantiate(prefab, groupTransform);
                platform.GetComponent<Platform>().SetPlatformGenerator(this);
                platform.name = prefab.name;

                return platform;
            },
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj));
    }
}
