using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BoosterGenerator : GeneratorBase
{
    [SerializeField] private GameObject _propellerPrefab;
    [SerializeField] private GameObject _jetpackPrefab;

    private IObjectPool<GameObject> _propellerPool;
    private IObjectPool<GameObject> _jetpackPool;

    private readonly List<GameObject> _activeBoosters = new();
    private PlatformGenerator _platformGenerator;

    private void Awake()
    {
        Transform boosterGroup = new GameObject("Boosters").transform;
        _propellerPool = CreatePool(_propellerPrefab, boosterGroup);
        _jetpackPool = CreatePool(_jetpackPrefab, boosterGroup);
    }

    public void SetPlatformGenerator(PlatformGenerator platformGenerator) => _platformGenerator = platformGenerator;

    public override void Generate(float height)
    {
        if (height < Settings.BoosterMinHeight)
            return;

        GameObject boosterPrefab = null;
        float random = Random.Range(0f, 1f);

        if (random < Settings.PropellerFrequency)
        {
            Vector2 platformPosition = _platformGenerator.SpawnNormalPlatform(height);
            boosterPrefab = _propellerPool.Get();
            boosterPrefab.transform.position = new Vector2(platformPosition.x, height + 0.3f);
        }
        else if (random < Settings.PropellerFrequency + Settings.JetpackFrequency)
        {
            Vector2 platformPosition = _platformGenerator.SpawnNormalPlatform(height);
            boosterPrefab = _jetpackPool.Get();
            boosterPrefab.transform.position = new Vector2(platformPosition.x, height + 0.4f);
        }

        if (boosterPrefab != null)
            _activeBoosters.Add(boosterPrefab);
    }

    public override void RemoveOffScreenElements(float cameraHeight)
    {
        for (int i = _activeBoosters.Count - 1; i >= 0; i--)
        {
            if (_activeBoosters[i].transform.position.y < cameraHeight - Camera.main.orthographicSize)
            {
                if (_activeBoosters[i].TryGetComponent<Propeller>(out _))
                    _propellerPool.Release(_activeBoosters[i]);
                else if (_activeBoosters[i].TryGetComponent<Jetpack>(out _))
                    _jetpackPool.Release(_activeBoosters[i]);

                _activeBoosters.RemoveAt(i);
            }
        }
    }

    public void ReleaseBooster(Booster booster)
    {
        GameObject boosterObject = booster.gameObject;

        if (booster is Propeller)
            _propellerPool.Release(boosterObject);
        else if (booster is Jetpack)
            _jetpackPool.Release(boosterObject);

        _activeBoosters.Remove(boosterObject);
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
