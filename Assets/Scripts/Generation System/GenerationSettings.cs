using UnityEngine;

[CreateAssetMenu(fileName = "GenerationSettings", menuName = "Game/Generation Settings")]
public class GenerationSettings : ScriptableObject
{
    [Header("Spacing between objects")]
    [SerializeField] private float _minimalSpacing = 0.3f;
    [SerializeField] private float _maximumSpacing = 2.6f;

    [Header("Platform Settings")]
    [SerializeField] private float _normalPlatformFrequency = 0.5f;
    [SerializeField] private float _breakablePlatformFrequency = 0.2f;
    [SerializeField] private float _movingPlatformFrequency = 0.2f;
    [SerializeField] private float _disappearingPlatformFrequency = 0.1f;
    [SerializeField] private float _movingPlatformMinHeight = 30f;
    [SerializeField] private float _disappearingPlatformMinHeight = 150f;

    [Header("Booster Settings")]
    [SerializeField] private float _propellerFrequency = 0.01f;
    [SerializeField] private float _jetpackFrequency = 0.008f;
    [SerializeField] private float _boosterMinHeight = 50f;

    [Header("Static Booster Settings")]
    [SerializeField] private float _springFrequency = 0.05f;

    [Header("Monster Settings")]
    [SerializeField] private float _flyingMonsterFrequency = 0.01f;
    [SerializeField] private float _walkingMonsterFrequency = 0.005f;
    [SerializeField] private float _holeFrequency = 0.005f;
    [SerializeField] private float _flyingMonsterMinHeight = 50f;
    [SerializeField] private float _walkingMonsterMinHeight = 100f;
    [SerializeField] private float _holeMinHeight = 150f;

    public float MinimalSpacing => _minimalSpacing;
    public float MaximumSpacing => _maximumSpacing;

    public float NormalPlatformFrequency => _normalPlatformFrequency;
    public float BreakablePlatformFrequency => _breakablePlatformFrequency;
    public float MovingPlatformFrequency => _movingPlatformFrequency;
    public float DisappearingPlatformFrequency => _disappearingPlatformFrequency;
    public float MovingPlatformMinHeight => _movingPlatformMinHeight;
    public float DisappearingPlatformMinHeight => _disappearingPlatformMinHeight;

    public float PropellerFrequency => _propellerFrequency;
    public float JetpackFrequency => _jetpackFrequency;
    public float BoosterMinHeight => _boosterMinHeight;

    public float SpringFrequency => _springFrequency;

    public float FlyingMonsterFrequency => _flyingMonsterFrequency;
    public float WalkingMonsterFrequency => _walkingMonsterFrequency;
    public float HoleFrequency => _holeFrequency;
    public float FlyingMonsterMinHeight => _flyingMonsterMinHeight;
    public float WalkingMonsterMinHeight => _walkingMonsterMinHeight;
    public float HoleMinHeight => _holeMinHeight;
}
