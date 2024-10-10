using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private GenerationSettings _generationSettings;
    [SerializeField] private PlatformGenerator _platformGenerator;
    [SerializeField] private MonsterGenerator _monsterGenerator;
    [SerializeField] private BoosterGenerator _boosterGenerator;
    [SerializeField] private PlayerMover _playerMover;

    private Camera _camera;
    private float _currentHeight = 0f;

    private void Awake() => _camera = Camera.main;

    private void OnEnable() => _playerMover.NewHeightReached += OnNewHeightReached;

    private void Start()
    {
        _platformGenerator.SetSettings(_generationSettings);
        _monsterGenerator.SetSettings(_generationSettings);
        _boosterGenerator.SetSettings(_generationSettings);

        _boosterGenerator.SetPlatformGenerator(_platformGenerator);

        GenerateInitialWorld();
    }

    private void OnDisable() => _playerMover.NewHeightReached -= OnNewHeightReached;

    private void GenerateInitialWorld()
    {
        Vector2 position = _platformGenerator.SpawnNormalPlatform(_currentHeight);
        _platformGenerator.PlacePlayerAboveFirstPlatform(position);
        _currentHeight += Random.Range(_generationSettings.MinimalSpacing, _generationSettings.MaximumSpacing);

        while (IsInCameraView(_currentHeight))
            GenerateNewElements();
    }

    private void OnNewHeightReached(float obj)
    {
        float cameraHeight = _camera.transform.position.y;

        while (IsInCameraView(_currentHeight))
            GenerateNewElements();

        RemoveOffScreenElements();
    }

    private void GenerateNewElements()
    {
        _platformGenerator.Generate(_currentHeight);
        _monsterGenerator.Generate(_currentHeight);
        _boosterGenerator.Generate(_currentHeight);

        _currentHeight += Random.Range(_generationSettings.MinimalSpacing, _generationSettings.MaximumSpacing);
    }

    private void RemoveOffScreenElements()
    {
        float cameraPositionY = _camera.transform.position.y;
        _platformGenerator.RemoveOffScreenElements(cameraPositionY);
        _monsterGenerator.RemoveOffScreenElements(cameraPositionY);
        _boosterGenerator.RemoveOffScreenElements(cameraPositionY);
    }

    private bool IsInCameraView(float height) => height < _camera.transform.position.y + 2 * _camera.orthographicSize;
}
