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
        float chance = Settings.PlatformFrequencyByHeight.Evaluate(height / Settings.MaxHeight);
        float rand = Random.Range(0f, 1f);
        GameObject platformPrefab;

        // Исправленная логика распределения вероятностей
        if (rand < Settings.NormalPlatformFrequency * chance)
        {
            platformPrefab = _normalPlatformPrefab;
        }
        else if (rand < (Settings.NormalPlatformFrequency + Settings.MovingPlatformFrequency) * chance)
        {
            platformPrefab = _movingPlatformPrefab;
        }
        else
        {
            platformPrefab = _disappearingPlatformPrefab;
        }

        // Спавн платформы
        GameObject platform = Instantiate(platformPrefab, GetRandomPosition(height), Quaternion.identity);
        _activePlatforms.Add(platform);
    }

    public Vector2 SpawnFirstPlatform()
    {
        GameObject platform = Instantiate(_normalPlatformPrefab);
        Vector2 position = new(GetRandomPositionX(), 0f);
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

    private float GetRandomPositionX() => Random.Range(-2.3f, 2.3f);

    private Vector2 GetRandomPosition(float height) => new(GetRandomPositionX(), height);
}
