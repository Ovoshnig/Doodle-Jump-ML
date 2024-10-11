using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private GenerationSettings _generationSettings;
    [SerializeField] private PlatformGenerator _platformGenerator;
    [SerializeField] private MonsterGenerator _monsterGenerator;
    [SerializeField] private BoosterGenerator _boosterGenerator;
    [SerializeField] private StaticBoosterGenerator _staticBoosterGenerator;

    private GeneratorBase[] _generators;
    private Camera _camera;
    private float _currentHeight = 0f;

    private void Awake() => _camera = Camera.main;

    private void Start()
    {
        _generators = new GeneratorBase[] 
        { 
            _platformGenerator, 
            _monsterGenerator, 
            _boosterGenerator 
        };

        foreach (var generator in _generators)
            generator.SetSettings(_generationSettings);

        _staticBoosterGenerator.SetSettings(_generationSettings);

        _platformGenerator.SetStaticBoosterGenerator(_staticBoosterGenerator);
        _boosterGenerator.SetPlatformGenerator(_platformGenerator);
        GenerateInitialWorld();
    }

    private void Update()
    {
        while (IsInCameraView(_currentHeight))
            GenerateNewElements();

        RemoveOffScreenElements();
    }

    private void GenerateInitialWorld()
    {
        Vector2 position = _platformGenerator.SpawnNormalPlatform(_currentHeight);
        _platformGenerator.PlacePlayerAboveFirstPlatform(position);
        _currentHeight += Random.Range(_generationSettings.MinimalSpacing, _generationSettings.MaximumSpacing);

        while (IsInCameraView(_currentHeight))
            GenerateNewElements();
    }

    private void GenerateNewElements()
    {
        foreach (var generator in _generators)
            generator.Generate(_currentHeight);

        _currentHeight += Random.Range(_generationSettings.MinimalSpacing, _generationSettings.MaximumSpacing);
    }

    private void RemoveOffScreenElements()
    {
        foreach(var generator in _generators)
            generator.RemoveOffScreenElements();

        _staticBoosterGenerator.RemoveOffScreenElements();
    }

    private bool IsInCameraView(float height) => height < _camera.transform.position.y + 2f * _camera.orthographicSize;
}
