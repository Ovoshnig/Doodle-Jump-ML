using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BoosterGenerator : GeneratorBase
{
    [SerializeField] private GameObject _propellerPrefab;
    [SerializeField] private GameObject _jetpackPrefab;
    [SerializeField] private Vector2 _propellerOffset = new(0.2f, 0.3f);
    [SerializeField] private Vector2 _jetpackOffset = new(0.2f, 0.4f);

    private IObjectPool<GameObject> _propellerPool;
    private IObjectPool<GameObject> _jetpackPool;
    private GameObject _lastActiveObject;

    protected override Transform GroupTransform { get; set; }

    protected override void Awake()
    {
        base.Awake();

        GroupTransform = new GameObject("Boosters").transform;
        _propellerPool = CreatePool(_propellerPrefab, GroupTransform);
        _jetpackPool = CreatePool(_jetpackPrefab, GroupTransform);

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

    public override void Generate(float height)
    {
        _lastActiveObject = null;

        if (ActiveObjects.Count > 0)
            return;

        IObjectPool<GameObject> pool;
        float random = Random.Range(0f, 1f);

        if (random < 2f * Settings.PropellerFrequency)
            pool = _propellerPool;
        else if (random < 2f * Settings.PropellerFrequency + 2f * Settings.JetpackFrequency)
            pool = _jetpackPool;
        else
            return;

        GameObject booster = pool.Get();
        ActiveObjects[booster] = pool;
        _lastActiveObject = booster;
    }

    public void PlaceBoosterAboutPlatform(GameObject platform)
    {
        Vector2 platformPosition = platform.transform.position;
        Generate(platformPosition.y);
        GameObject booster = _lastActiveObject;

        if (booster == null)
            return;

        booster.transform.SetParent(platform.transform);

        float signX = Random.Range(0, 2) == 0 ? -1 : 1;

        if (booster.name == _propellerPrefab.name)
        {
            _propellerOffset.x = signX * Mathf.Abs(_propellerOffset.x);
            booster.transform.localPosition = _propellerOffset;
        }
        else if (booster.name == _jetpackPrefab.name)
        {
            _jetpackOffset.x = signX * Mathf.Abs(_jetpackOffset.x);
            booster.transform.localPosition = _jetpackOffset;
        }
    }

    public void ReleaseBooster(Booster booster)
    {
        GameObject boosterObject = booster.gameObject;
        boosterObject.transform.SetParent(GroupTransform);

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
