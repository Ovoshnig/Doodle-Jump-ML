using UnityEngine;

public class PlayerJetpackView : PlayerBoosterView
{
    private const string SpeedMultiplierName = "speedMultiplier";

    [SerializeField] private AnimationClip _runningClip;

    protected override void RunAnimation(float targetDuration)
    {
        base.RunAnimation(targetDuration);
        Animator.SetFloat(SpeedMultiplierName, _runningClip.length / targetDuration);
    }
}
