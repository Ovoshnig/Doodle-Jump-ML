using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private GenerationSettings _generationSettings;
    [SerializeField] private PlatformGenerator _platformGenerator;
    [SerializeField] private MonsterGenerator _monsterGenerator;
    [SerializeField] private BoosterGenerator _boosterGenerator;

    private float _currentHeight = 0f;

    private void Start()
    {
        _platformGenerator.SetSettings(_generationSettings);
        _monsterGenerator.SetSettings(_generationSettings);
        _boosterGenerator.SetSettings(_generationSettings);

        GenerateInitialWorld();
    }

    private void GenerateInitialWorld()
    {
        Vector2 position = _platformGenerator.SpawnFirstPlatform();
        _platformGenerator.PlacePlayerAboveFirstPlatform(position);

        while (_currentHeight < _generationSettings.MaxHeight)
        {
            _platformGenerator.Generate(_currentHeight);
            _monsterGenerator.Generate(_currentHeight);
            _boosterGenerator.Generate(_currentHeight);

            // Увеличиваем текущую высоту на случайное значение
            _currentHeight += Random.Range(_generationSettings.MinimalSpacing, _generationSettings.MaximumSpacing);
        }
    }
}
