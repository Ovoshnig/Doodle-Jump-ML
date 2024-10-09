using UnityEngine;

public class MonsterGenerator : GeneratorBase
{
    [SerializeField] private GameObject _flyingMonsterPrefab;
    [SerializeField] private GameObject _walkingMonsterPrefab;
    [SerializeField] private GameObject _blackHolePrefab;

    public override void Generate(float height)
    {
        // ѕровер€ем минимальные высоты дл€ генерации монстров
        if (height < Settings.FlyingMonsterStartHeight && height < Settings.WalkingMonsterStartHeight && height < Settings.BlackHoleStartHeight)
        {
            return;
        }

        // √енераци€ монстра в зависимости от высоты
        if (height >= Settings.BlackHoleStartHeight && Random.Range(0f, 1f) < Settings.MonsterFrequencyByHeight.Evaluate(height / Settings.MaxHeight))
        {
            Instantiate(_blackHolePrefab, GetRandomPosition(height), Quaternion.identity);
        }
        else if (height >= Settings.WalkingMonsterStartHeight)
        {
            Instantiate(_walkingMonsterPrefab, GetRandomPosition(height), Quaternion.identity);
        }
        else if (height >= Settings.FlyingMonsterStartHeight)
        {
            Instantiate(_flyingMonsterPrefab, GetRandomPosition(height), Quaternion.identity);
        }
    }

    private Vector2 GetRandomPosition(float height)
    {
        float x = Random.Range(-2.5f, 2.5f); // ќграничени€ по X
        return new Vector2(x, height);
    }
}
