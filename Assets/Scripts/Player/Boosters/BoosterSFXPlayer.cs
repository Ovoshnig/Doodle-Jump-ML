using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BoosterSFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] _clips;

    private AudioSource _audioSource;
    private BoosterLogic _boosterLogic;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
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

    private void OnStarted()
    {
        AudioClip clip = GetRandomClip();
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    private void OnStopped()
    {
        _audioSource.Stop();
        _audioSource.clip = null;
    }

    private AudioClip GetRandomClip()
    {
        int index = Random.Range(0, _clips.Length);

        return _clips[index];
    }
}
