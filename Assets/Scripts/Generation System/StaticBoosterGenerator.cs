using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class StaticBoosterGenerator : GeneratorBase
{
    [SerializeField] private GameObject _springPrefab;
    [SerializeField] private Vector2 _springOffset = new(0.2f, 0.13f);

    private IObjectPool<GameObject> _springPool;
    private GameObject _lastActiveObject;

    protected override Transform GroupTransform { get; set; }

    protected override void Awake()
    {
        base.Awake();

        GroupTransform = new GameObject("Static Boosters").transform;
        _springPool = CreatePool(_springPrefab, GroupTransform);

        List<GameObject> objects = new()
        {
            _springPrefab
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

    public override void Generate(ref float height)
    {
        _lastActiveObject = null;
        float random = Random.Range(0f, 1f);

        if (random > 2f * Settings.SpringFrequency)
            return;

        GameObject spring = _springPool.Get();
        height += ObjectHalfSizesY[spring.name];
        _lastActiveObject = spring;
        ActiveObjects[spring] = _springPool;
    }

    public void PlaceStaticBoosterAboutPlatform(GameObject platform)
    {
        Vector2 platformPosition = platform.transform.position;
        Generate(ref platformPosition.y);
        GameObject spring = _lastActiveObject;

        if (spring == null)
            return;

        spring.transform.SetParent(platform.transform);

        float signX = Random.Range(0, 2) == 0 ? -1 : 1;
        _springOffset.x = signX * Mathf.Abs(_springOffset.x);
        spring.transform.localPosition = _springOffset;
    }

    private ObjectPool<GameObject> CreatePool(GameObject prefab, Transform groupTransform)
    {
        return new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject staticBooster = Instantiate(prefab, groupTransform);
                staticBooster.name = prefab.name;

                return staticBooster;
            },
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj));
    }
}
