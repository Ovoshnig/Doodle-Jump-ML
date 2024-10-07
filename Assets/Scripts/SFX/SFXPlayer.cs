using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _jumpClip;
    [SerializeField] private AudioClip _loseClip;
    
    private AudioSource _audioSource;
    private PlayerMover _playerMover;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerMover = GetComponent<PlayerMover>();

        _playerMover.PlatformJumpedOff += OnPlatformJumpedOff;
        _playerMover.Lost += OnLost;
    }

    private void OnDestroy()
    {
        _playerMover.PlatformJumpedOff -= OnPlatformJumpedOff;
        _playerMover.Lost -= OnLost;
    }

    private void OnPlatformJumpedOff()
    {
        _audioSource.clip = _jumpClip;
        _audioSource.Play();
    }

    private void OnLost()
    {
        _audioSource.clip = _loseClip;
        _audioSource.Play();
    }
}
