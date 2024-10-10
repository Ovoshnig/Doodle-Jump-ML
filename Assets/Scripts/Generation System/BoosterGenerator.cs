using UnityEngine;

public class BoosterGenerator : GeneratorBase
{
    [SerializeField] private GameObject _propellerPrefab;
    [SerializeField] private GameObject _jetpackPrefab;

    private PlatformGenerator _platformGenerator;

    public void SetPlatformGenerator(PlatformGenerator platformGenerator) => _platformGenerator = platformGenerator;

    public override void Generate(float height)
    {
        if (height < Settings.BoosterMinHeight)
            return;

        float random = Random.Range(0f, 1f);

        if (random < Settings.PropellerFrequency)
        {
            Vector2 platformPosition = _platformGenerator.SpawnNormalPlatform(height);
            Instantiate(_propellerPrefab, new Vector2(platformPosition.x, height + 0.3f), Quaternion.identity);
        }
        else if (random < Settings.PropellerFrequency + Settings.JetpackFrequency)
        {
            Vector2 platformPosition = _platformGenerator.SpawnNormalPlatform(height);
            Instantiate(_jetpackPrefab, new Vector2(platformPosition.x, height + 0.4f), Quaternion.identity);
        }
    }

    protected override Vector2 GetRandomPosition(float height)
    {
        float x = Random.Range(-2.5f, 2.5f); // Ограничения по X
        return new Vector2(x, height);
    }
}
