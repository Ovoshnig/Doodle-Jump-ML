using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _jumpClip;
    [SerializeField] private AudioClip _loseClip;
    [SerializeField] private AudioClip _monsterDownClip;
    
    private AudioSource _audioSource;
    private PlayerMover _playerMover;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerMover = GetComponent<PlayerMover>();

        _playerMover.PlatformJumpedOff += OnPlatformJumpedOff;
        _playerMover.Lost += OnLost;
        _playerMover.MonsterDowned += OnMonsterDowned;
    }


    private void OnDestroy()
    {
        _playerMover.PlatformJumpedOff -= OnPlatformJumpedOff;
        _playerMover.Lost -= OnLost;
        _playerMover.MonsterDowned -= OnMonsterDowned;
    }

    private void OnPlatformJumpedOff()
    {
        _audioSource.clip = _jumpClip;
        _audioSource.Play();
    }

    private void OnLost()
    {
        if (_audioSource.clip != _loseClip)
        {
            _audioSource.clip = _loseClip;
            _audioSource.Play();
        }
    }

    private void OnMonsterDowned()
    {
        _audioSource.clip = _monsterDownClip;
        _audioSource.Play();
    }
}
