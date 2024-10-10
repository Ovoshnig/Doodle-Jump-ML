using UnityEngine;

public class MonsterGenerator : GeneratorBase
{
    [SerializeField] private GameObject _flyingMonster1Prefab;
    [SerializeField] private GameObject _flyingMonster2Prefab;
    [SerializeField] private GameObject _walkingMonsterPrefab;
    [SerializeField] private GameObject _holePrefab;

    public override void Generate(float height)
    {
        float random = Random.Range(0f, 1f);

        if (random < Settings.FlyingMonsterFrequency && height > Settings.FlyingMonsterMinHeight)
        {
            GameObject flyingMonsterPrefab = Random.Range(0, 2) == 0 ? _flyingMonster1Prefab : _flyingMonster2Prefab;
            Instantiate(flyingMonsterPrefab, GetRandomPosition(height), Quaternion.identity);
        }
        else if (random < Settings.FlyingMonsterFrequency + Settings.WalkingMonsterFrequency 
            && height > Settings.WalkingMonsterMinHeight)
        {
            Instantiate(_walkingMonsterPrefab, GetRandomPosition(height), Quaternion.identity);
        }
        else if (random < Settings.FlyingMonsterFrequency + Settings.WalkingMonsterFrequency + Settings.HoleFrequency
            && height > Settings.HoleMinHeight)
        {
            Instantiate(_holePrefab, GetRandomPosition(height), Quaternion.identity);
        }
    }

    protected override Vector2 GetRandomPosition(float height)
    {
        float x = Random.Range(-2.5f, 2.5f);

        return new Vector2(x, height);
    }
}
