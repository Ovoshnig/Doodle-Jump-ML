using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class StaticBoosterGenerator : GeneratorBase
{
    [SerializeField] private GameObject _springPrefab;

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

    public override void Generate(float height)
    {
        GameObject spring = _springPool.Get();
        ActiveObjects[spring] = _springPool;
        _lastActiveObject = spring;
    }

    public void PlaceStaticBoosterAboutPlatform(GameObject platform)
    {
        Vector2 platformPosition = platform.transform.position;
        Generate(platformPosition.y);
        GameObject spring = _lastActiveObject;
        spring.transform.SetParent(platform.transform);
        spring.transform.localPosition = new Vector2(0.2f, 0.12f);
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
