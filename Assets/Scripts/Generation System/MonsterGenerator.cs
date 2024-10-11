using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MonsterGenerator : GeneratorBase
{
    [SerializeField] private GameObject _flyingMonster1Prefab;
    [SerializeField] private GameObject _flyingMonster2Prefab;
    [SerializeField] private GameObject _walkingMonsterPrefab;
    [SerializeField] private GameObject _holePrefab;

    private IObjectPool<GameObject> _flyingMonsterPool;
    private IObjectPool<GameObject> _walkingMonsterPool;
    private IObjectPool<GameObject> _holePool;

    protected override void Awake()
    {
        base.Awake();

        Transform monsterGroup = new GameObject("Monsters").transform;
        int random = Random.Range(0, 2);
        _flyingMonsterPool = CreatePool(random == 0 ? _flyingMonster1Prefab : _flyingMonster2Prefab, monsterGroup);
        _walkingMonsterPool = CreatePool(_walkingMonsterPrefab, monsterGroup);
        _holePool = CreatePool(_holePrefab, monsterGroup);

        List<GameObject> objects = new()
        {
            _flyingMonster1Prefab,
            _flyingMonster2Prefab,
            _walkingMonsterPrefab,
            _holePrefab
        };

        float screenBoundX = Camera.ViewportToWorldPoint(new Vector2(1f, 0f)).x;

        foreach (var @object in objects)
        {
            if (@object.TryGetComponent(out CapsuleCollider2D capsuleCollider))
                ObjectBoundsX[@object.name] = screenBoundX - 0.5f * capsuleCollider.size.x * @object.transform.lossyScale.x;
            else if (@object.TryGetComponent(out CircleCollider2D circleCollider))
                ObjectBoundsX[@object.name] = screenBoundX - circleCollider.radius * @object.transform.lossyScale.x;
        }
    }

    public override void Generate(float height)
    {
        if (ActiveObjects.Count > 0)
            return;

        IObjectPool<GameObject> pool;
        float random = Random.Range(0f, 1f);

        if (random < Settings.FlyingMonsterFrequency && height > Settings.FlyingMonsterMinHeight)
            pool = _flyingMonsterPool;
        else if (random < Settings.FlyingMonsterFrequency + Settings.WalkingMonsterFrequency
            && height > Settings.WalkingMonsterMinHeight)
            pool = _walkingMonsterPool;
        else if (random < Settings.FlyingMonsterFrequency + Settings.WalkingMonsterFrequency + Settings.HoleFrequency
            && height > Settings.HoleMinHeight)
            pool = _holePool;
        else
            return;

        GameObject monster = pool.Get();
        ActiveObjects[monster] = pool;
        monster.transform.position = GetRandomPosition(monster, height);
    }

    private Vector2 GetRandomPosition(GameObject @object, float height)
    {
        float boundX = ObjectBoundsX[@object.name];
        float x = Random.Range(-boundX, boundX);

        return new Vector2(x, height);
    }

    private ObjectPool<GameObject> CreatePool(GameObject prefab, Transform groupTransform)
    {
        return new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject monster = Instantiate(prefab, groupTransform);
                monster.name = prefab.name;

                return monster;
            },
            actionOnGet: hole => hole.SetActive(true),
            actionOnRelease: hole => hole.SetActive(false),
            actionOnDestroy: Destroy
        );
    }
}
