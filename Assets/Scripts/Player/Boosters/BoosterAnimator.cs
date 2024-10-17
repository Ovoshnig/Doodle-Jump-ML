using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BoosterAnimator : MonoBehaviour
{
    private const string IsRunningName = "isRunning";

    private Animator _animator;
    private BoosterLogic _boosterLogic;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _boosterLogic = GetComponent<BoosterLogic>();
    }

    private void OnEnable()
    {
        _boosterLogic.Launched += OnStarted;
        _boosterLogic.Stopped += OnStopped;
    }

    private void OnDisable()
    {
        _boosterLogic.Launched -= OnStarted;
        _boosterLogic.Stopped -= OnStopped;
    }

    private void OnStarted() => _animator.SetBool(IsRunningName, true);

    private void OnStopped() => _animator.SetBool(IsRunningName, false);
}
