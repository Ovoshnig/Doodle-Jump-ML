using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BoosterGenerator : GeneratorBase
{
    [SerializeField] private GameObject _propellerPrefab;
    [SerializeField] private GameObject _jetpackPrefab;

    private IObjectPool<GameObject> _propellerPool;
    private IObjectPool<GameObject> _jetpackPool;
    private PlatformGenerator _platformGenerator;

    protected override void Awake()
    {
        base.Awake();

        Transform boosterGroup = new GameObject("Boosters").transform;
        _propellerPool = CreatePool(_propellerPrefab, boosterGroup);
        _jetpackPool = CreatePool(_jetpackPrefab, boosterGroup);

        List<GameObject> objects = new()
        {
            _propellerPrefab,
            _jetpackPrefab
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

    public void SetPlatformGenerator(PlatformGenerator platformGenerator) => _platformGenerator = platformGenerator;

    public override void Generate(float height)
    {
        if (ActiveObjects.Count > 0 || height < Settings.BoosterMinHeight)
            return;

        IObjectPool<GameObject> pool;
        float verticalOffset;
        float random = Random.Range(0f, 1f);

        if (random < Settings.PropellerFrequency)
        {
            pool = _propellerPool;
            verticalOffset = 0.3f;
        }
        else if (random < Settings.PropellerFrequency + Settings.JetpackFrequency)
        {
            pool = _jetpackPool;
            verticalOffset = 0.4f;
        }
        else
        {
            return;
        }

        GameObject booster = pool.Get();
        ActiveObjects[booster] = pool;
        Vector2 platformPosition = _platformGenerator.SpawnNormalPlatform(height);
        booster.transform.position = new Vector2(platformPosition.x, height + verticalOffset);
    }

    public void ReleaseBooster(Booster booster)
    {
        GameObject boosterObject = booster.gameObject;

        if (booster is Propeller)
            _propellerPool.Release(boosterObject);
        else if (booster is Jetpack)
            _jetpackPool.Release(boosterObject);

        ActiveObjects.Remove(boosterObject);
    }

    private ObjectPool<GameObject> CreatePool(GameObject prefab, Transform groupTransform)
    {
        return new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject booster = Instantiate(prefab, groupTransform);
                booster.GetComponent<Booster>().SetBoosterGenerator(this);
                booster.name = prefab.name;

                return booster;
            },
            actionOnGet: booster => booster.SetActive(true),
            actionOnRelease: booster => booster.SetActive(false),
            actionOnDestroy: Destroy
        );
    }
}
