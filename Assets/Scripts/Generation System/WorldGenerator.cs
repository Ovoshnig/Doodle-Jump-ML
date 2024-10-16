using System;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private GenerationSettings _generationSettings;
    [SerializeField] private PlatformGenerator _platformGenerator;
    [SerializeField] private MonsterGenerator _monsterGenerator;
    [SerializeField] private BoosterGenerator _boosterGenerator;
    [SerializeField] private StaticBoosterGenerator _staticBoosterGenerator;
    [SerializeField] private PlayerMover _playerMover;

    private GeneratorBase[] _generators;
    private Camera _camera;
    private float _currentHeight = 0f;

    private void Awake() => _camera = Camera.main;

    private void OnEnable() => _playerMover.EpisodeBegan += OnEpisodeBegan;

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

        _boosterGenerator.SetSettings(_generationSettings);
        _staticBoosterGenerator.SetSettings(_generationSettings);

        _platformGenerator.SetBoosterGenerator(_boosterGenerator);
        _platformGenerator.SetStaticBoosterGenerator(_staticBoosterGenerator);
    }

    private void OnDisable() => _playerMover.EpisodeBegan -= OnEpisodeBegan;

    private void Update()
    {
        while (IsInCameraView(_currentHeight))
            GenerateNewElements();

        RemoveOffScreenElements();
    }

    private void OnEpisodeBegan()
    {
        foreach (var generator in _generators)
            generator.ReleaseAllActiveElements();

        _currentHeight = 0f;
        Vector2 position = _platformGenerator.SpawnNormalPlatform(ref _currentHeight);
        _platformGenerator.PlacePlayerAboveFirstPlatform(position);
    }

    private void GenerateNewElements()
    {
        foreach (var generator in _generators)
            generator.Generate(ref _currentHeight);
    }

    private void RemoveOffScreenElements()
    {
        foreach(var generator in _generators)
            generator.ReleaseOffScreenElements();

        _boosterGenerator.ReleaseOffScreenElements();
        _staticBoosterGenerator.ReleaseOffScreenElements();
    }

    private bool IsInCameraView(float height) => height < _camera.transform.position.y + 2f * _camera.orthographicSize;
}
