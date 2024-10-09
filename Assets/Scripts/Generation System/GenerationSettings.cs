using UnityEngine;

[CreateAssetMenu(fileName = "GenerationSettings", menuName = "Game/Generation Settings")]
public class GenerationSettings : ScriptableObject
{
    [field: SerializeField] public float MinimalSpacing { get; private set; } = 0.3f;
    [field: SerializeField] public float MaximumSpacing { get; private set; } = 2.6f;

    [field: SerializeField] public float NormalPlatformFrequency { get; private set; } = 0.7f;
    [field: SerializeField] public float MovingPlatformFrequency { get; private set; } = 0.2f;
    [field: SerializeField] public float DisappearingPlatformFrequency { get; private set; } = 0.1f;

    [field: SerializeField] public float FlyingMonsterStartHeight { get; private set; } = 50f;
    [field: SerializeField] public float WalkingMonsterStartHeight { get; private set; } = 100f;
    [field: SerializeField] public float BlackHoleStartHeight { get; private set; } = 150f;

    [field: SerializeField] public float PropellerFrequency { get; private set; } = 0.05f;
    [field: SerializeField] public float JetpackFrequency { get; private set; } = 0.03f;
    [field: SerializeField] public float BoosterMinHeight { get; private set; } = 30f;

    [field: SerializeField] public float MaxHeight { get; private set; } = 500f;
    [field: SerializeField] public AnimationCurve PlatformFrequencyByHeight { get; private set; }
    [field: SerializeField] public AnimationCurve MonsterFrequencyByHeight { get; private set; }
    [field: SerializeField] public AnimationCurve BoosterFrequencyByHeight { get; private set; }
}
