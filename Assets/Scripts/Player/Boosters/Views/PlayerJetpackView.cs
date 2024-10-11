using UnityEngine;

public class PlayerJetpackView : PlayerBoosterView
{
    private const string SpeedMultiplierName = "speedMultiplier";

    [SerializeField] private AnimationClip _runningClip;

    protected override void PlayRunningAnimation(float targetDuration)
    {
        base.PlayRunningAnimation(targetDuration);
        Animator.SetFloat(SpeedMultiplierName, _runningClip.length / targetDuration);
    }
}
