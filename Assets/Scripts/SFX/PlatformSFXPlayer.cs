using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlatformSFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _jumpClip;
    
    private AudioSource _audioSource;
    private PlayerMover _playerMover;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerMover = GetComponent<PlayerMover>();
        _playerMover.PlatformJumpedOff += OnPlatformJumpedOff;
    }

    private void OnDestroy() => _playerMover.PlatformJumpedOff -= OnPlatformJumpedOff;

    private void OnPlatformJumpedOff()
    {
        _audioSource.clip = _jumpClip;
        _audioSource.Play();
    }
}
