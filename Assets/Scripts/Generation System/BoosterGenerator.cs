using UnityEngine;

public class BoosterGenerator : GeneratorBase
{
    [SerializeField] private GameObject _propellerPrefab;
    [SerializeField] private GameObject _jetpackPrefab;

    public override void Generate(float height)
    {
        if (height < Settings.BoosterMinHeight)
            return;

        float chance = Settings.BoosterFrequencyByHeight.Evaluate(height / Settings.MaxHeight);
        float random = Random.Range(0f, 1f);

        if (random < Settings.PropellerFrequency * chance)
        {
            Instantiate(_propellerPrefab, GetRandomPosition(height), Quaternion.identity);
        }
        else if (random < (Settings.PropellerFrequency + Settings.JetpackFrequency) * chance)
        {
            Instantiate(_jetpackPrefab, GetRandomPosition(height), Quaternion.identity);
        }
    }

    private Vector2 GetRandomPosition(float height)
    {
        float x = Random.Range(-2.5f, 2.5f); // Ограничения по X
        return new Vector2(x, height);
    }
}
