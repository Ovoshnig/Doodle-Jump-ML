using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _jumpClip;
    [SerializeField] private AudioClip _loseClip;
    [SerializeField] private AudioClip _crashedIntoMonsterClip;
    [SerializeField] private AudioClip _flewIntoHoleClip;
    [SerializeField] private AudioClip _monsterDownClip;
    
    private AudioSource _audioSource;
    private PlayerMover _playerMover;
    private PlayerCollisionHandler _collisionHandler;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerMover = GetComponent<PlayerMover>();
        _collisionHandler = GetComponent<PlayerCollisionHandler>();

        _collisionHandler.PlatformJumpedOff += OnPlatformJumpedOff;
        _collisionHandler.MonsterDowned += OnMonsterDowned;
        _collisionHandler.CrashedIntoMonster += OnCrashedIntoMonster;
        _collisionHandler.FlewIntoHole += OnFlewIntoHole;
        _playerMover.Lost += OnLost;
    }

    private void OnDestroy()
    {
        _collisionHandler.PlatformJumpedOff -= OnPlatformJumpedOff;
        _collisionHandler.MonsterDowned -= OnMonsterDowned;
        _collisionHandler.CrashedIntoMonster -= OnCrashedIntoMonster;
        _collisionHandler.FlewIntoHole -= OnFlewIntoHole;
        _playerMover.Lost -= OnLost;
    }

    private void OnPlatformJumpedOff(float height)
    {
        _audioSource.clip = _jumpClip;
        _audioSource.Play();
    }

    private void OnMonsterDowned(float height)
    {
        _audioSource.clip = _monsterDownClip;
        _audioSource.PlayOneShot(_monsterDownClip);
    }

    private void OnCrashedIntoMonster() => _audioSource.PlayOneShot(_crashedIntoMonsterClip, 1.5f);

    private void OnFlewIntoHole() => _audioSource.PlayOneShot(_flewIntoHoleClip, 1.5f);

    private void OnLost()
    {
        _audioSource.clip = _loseClip;
        _audioSource.Play();
    }
}
