using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : GeneratorBase
{
    [SerializeField] private GameObject _normalPlatformPrefab;
    [SerializeField] private GameObject _movingPlatformPrefab;
    [SerializeField] private GameObject _disappearingPlatformPrefab;
    [SerializeField] private GameObject _player;

    private readonly List<GameObject> _activePlatforms = new();

    public override void Generate(float height)
    {
        float random = Random.Range(0f, 1f);
        GameObject platformPrefab;

        if (random < Settings.NormalPlatformFrequency)
            platformPrefab = _normalPlatformPrefab;
        else if (random < Settings.NormalPlatformFrequency + Settings.MovingPlatformFrequency && height > Settings.MovingPlatformMinHeight)
            platformPrefab = _movingPlatformPrefab;
        else if (height > Settings.DisappearingPlatformMinHeight)
            platformPrefab = _disappearingPlatformPrefab;
        else
            platformPrefab = _normalPlatformPrefab;

        GameObject platform = Instantiate(platformPrefab, GetRandomPosition(height), Quaternion.identity);
        _activePlatforms.Add(platform);
    }

    public Vector2 SpawnNormalPlatform(float height)
    {
        GameObject platform = Instantiate(_normalPlatformPrefab);
        Vector2 position = new(GetRandomPositionX(), height);
        platform.transform.position = position;
        _activePlatforms.Add(platform);

        return position;
    }

    public void PlacePlayerAboveFirstPlatform(Vector2 platformPosition) => 
        _player.transform.position = platformPosition + Vector2.up * 0.6f;

    public void DisablePlatformsBelowCamera()
    {
        // Отключаем платформы ниже камеры
    }

    protected override Vector2 GetRandomPosition(float height) => new(GetRandomPositionX(), height);

    private float GetRandomPositionX() => Random.Range(-2.3f, 2.3f);
}
