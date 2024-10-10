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
    }

    public override void Generate(float height)
    {
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
        monster.transform.position = GetRandomPosition(height);
    }

    protected override Vector2 GetRandomPosition(float height)
    {
        float x = Random.Range(-2.5f, 2.5f);

        return new Vector2(x, height);
    }

    private ObjectPool<GameObject> CreatePool(GameObject prefab, Transform groupTransform)
    {
        return new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab, groupTransform),
            actionOnGet: hole => hole.SetActive(true),
            actionOnRelease: hole => hole.SetActive(false),
            actionOnDestroy: Destroy
        );
    }
}
