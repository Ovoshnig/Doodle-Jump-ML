using UnityEngine;

[CreateAssetMenu(fileName = "GenerationSettings", menuName = "Game/Generation Settings")]
public class GenerationSettings : ScriptableObject
{
    [Header("Spacing between objects")]
    [field: SerializeField] public float MinimalSpacing { get; private set; } = 0.3f;
    [field: SerializeField] public float MaximumSpacing { get; private set; } = 2.6f;

    [Header("Platform Settings")]
    [field: SerializeField] public float NormalPlatformFrequency { get; private set; } = 0.7f;
    [field: SerializeField] public float MovingPlatformFrequency { get; private set; } = 0.2f;
    [field: SerializeField] public float DisappearingPlatformFrequency { get; private set; } = 0.1f;
    [field: SerializeField] public float MovingPlatformMinHeight { get; private set; } = 20f;
    [field: SerializeField] public float DisappearingPlatformMinHeight { get; private set; } = 150f;

    [Header("Static Booster Settings")]
    [field: SerializeField] public float SpringOnNormalPlatformFrequency { get; private set; } = 0.05f;
    [field: SerializeField] public float SpringOnMovingPlatformFrequency { get; private set; } = 0.03f;

    [Header("Monster Settings")]
    [field: SerializeField] public float FlyingMonsterFrequency { get; private set; } = 0.01f;
    [field: SerializeField] public float WalkingMonsterFrequency { get; private set; } = 0.005f;
    [field: SerializeField] public float HoleFrequency { get; private set; } = 0.005f;
    [field: SerializeField] public float FlyingMonsterMinHeight { get; private set; } = 50f;
    [field: SerializeField] public float WalkingMonsterMinHeight { get; private set; } = 100f;
    [field: SerializeField] public float HoleMinHeight { get; private set; } = 150f;

    [Header("Booster Settings")]
    [field: SerializeField] public float PropellerFrequency { get; private set; } = 0.05f;
    [field: SerializeField] public float JetpackFrequency { get; private set; } = 0.03f;
    [field: SerializeField] public float BoosterMinHeight { get; private set; } = 30f;
}
