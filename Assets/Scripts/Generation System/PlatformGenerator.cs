using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlatformGenerator : GeneratorBase
{
    [SerializeField] private GameObject _normalPlatformPrefab;
    [SerializeField] private GameObject _movingPlatformPrefab;
    [SerializeField] private GameObject _disappearingPlatformPrefab;
    [SerializeField] private GameObject _player;

    private BoosterGenerator _boosterGenerator;
    private StaticBoosterGenerator _staticBoosterGenerator;
    private IObjectPool<GameObject> _normalPlatformPool;
    private IObjectPool<GameObject> _movingPlatformPool;
    private IObjectPool<GameObject> _disappearingPlatformPool;

    protected override Transform GroupTransform { get; set; }

    protected override void Awake()
    {
        base.Awake();

        GroupTransform = new GameObject("Platforms").transform;
        _normalPlatformPool = CreatePool(_normalPlatformPrefab, GroupTransform);
        _movingPlatformPool = CreatePool(_movingPlatformPrefab, GroupTransform);
        _disappearingPlatformPool = CreatePool(_disappearingPlatformPrefab, GroupTransform);

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

    public void SetBoosterGenerator(BoosterGenerator generator) => 
        _boosterGenerator = generator;

    public void SetStaticBoosterGenerator(StaticBoosterGenerator generator) => 
        _staticBoosterGenerator = generator;

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
        Vector2 platformPosition = GetRandomPosition(platform, height);
        platform.transform.position = platformPosition;

        if (pool != _normalPlatformPool && pool != _movingPlatformPool)
            return;

        random = Random.Range(0f, 1f);

        if (height > Settings.BoosterMinHeight && random < 0.5f)
            _boosterGenerator.PlaceBoosterAboutPlatform(platform);
        else
            _staticBoosterGenerator.PlaceStaticBoosterAboutPlatform(platform);
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

    public void ReleaseDisappearingPlatform(DisappearingPlatform platform)
    {
        GameObject platformObject = platform.gameObject;
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
