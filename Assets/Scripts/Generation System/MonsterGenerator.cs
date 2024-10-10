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

    private readonly List<GameObject> _activeMonsters = new();

    private void Awake()
    {
        Transform monsterGroup = new GameObject("Monsters").transform;
        int random = Random.Range(0, 2);
        _flyingMonsterPool = CreatePool(random == 0 ? _flyingMonster1Prefab : _flyingMonster2Prefab, monsterGroup);
        _walkingMonsterPool = CreatePool(_walkingMonsterPrefab, monsterGroup);
        _holePool = CreatePool(_holePrefab, monsterGroup);
    }

    public override void Generate(float height)
    {
        GameObject monsterPrefab = null;
        float random = Random.Range(0f, 1f);

        if (random < Settings.FlyingMonsterFrequency && height > Settings.FlyingMonsterMinHeight)
            monsterPrefab = _flyingMonsterPool.Get();
        else if (random < Settings.FlyingMonsterFrequency + Settings.WalkingMonsterFrequency
            && height > Settings.WalkingMonsterMinHeight)
            monsterPrefab = _walkingMonsterPool.Get();
        else if (random < Settings.FlyingMonsterFrequency + Settings.WalkingMonsterFrequency + Settings.HoleFrequency
            && height > Settings.HoleMinHeight)
            monsterPrefab = _holePool.Get();

        if (monsterPrefab != null)
        {
            monsterPrefab.transform.position = GetRandomPosition(height);
            _activeMonsters.Add(monsterPrefab);
        }
    }

    public override void RemoveOffScreenElements(float cameraHeight)
    {
        for (int i = _activeMonsters.Count - 1; i >= 0; i--)
        {
            if (_activeMonsters[i].transform.position.y < cameraHeight - Camera.main.orthographicSize)
            {
                if (_activeMonsters[i].TryGetComponent<FlyingMonster>(out _))
                    _flyingMonsterPool.Release(_activeMonsters[i]);
                else if (_activeMonsters[i].TryGetComponent<WalkingMonster>(out _))
                    _walkingMonsterPool.Release(_activeMonsters[i]);
                else if (_activeMonsters[i].TryGetComponent<Hole>(out _))
                    _holePool.Release(_activeMonsters[i]);

                _activeMonsters.RemoveAt(i);
            }
        }
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
