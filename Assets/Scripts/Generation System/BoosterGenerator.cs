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
    }

    public void SetPlatformGenerator(PlatformGenerator platformGenerator) => _platformGenerator = platformGenerator;

    public override void Generate(float height)
    {
        if (height < Settings.BoosterMinHeight)
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

    protected override Vector2 GetRandomPosition(float height)
    {
        float x = Random.Range(-2.5f, 2.5f);

        return new Vector2(x, height);
    }

    private ObjectPool<GameObject> CreatePool(GameObject prefab, Transform groupTransform)
    {
        return new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject booster = Instantiate(prefab, groupTransform);
                booster.GetComponent<Booster>().SetBoosterGenerator(this);

                return booster;
            },
            actionOnGet: booster => booster.SetActive(true),
            actionOnRelease: booster => booster.SetActive(false),
            actionOnDestroy: Destroy
        );
    }
}
