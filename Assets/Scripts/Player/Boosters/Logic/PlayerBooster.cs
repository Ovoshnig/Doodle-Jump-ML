using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class PlayerBooster : MonoBehaviour
{
    [SerializeField] private float _distance;
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Rigidbody2D _playerRigidbody;

    private AudioSource _audioSource;

    public bool IsRunning { get; private set; } = false;

    protected virtual void Awake() => _audioSource = GetComponent<AudioSource>();

    public float Run()
    {
        AudioClip clip = GetRandomClip();
        _audioSource.clip = clip;
        _audioSource.Play();
        float duration = clip.length;
        StartCoroutine(BoostRoutine(duration));

        return duration;
    }

    private IEnumerator BoostRoutine(float duration)
    {
        float speed = _distance / duration;
        float startPositionY = _playerTransform.position.y;
        float endPositionY = startPositionY + _distance;
        IsRunning = true;

        while (_playerTransform.position.y < endPositionY)
        {
            _playerRigidbody.linearVelocityY = speed;

            yield return null;
        }

        _playerRigidbody.linearVelocityY = 0f;
        IsRunning = false;
    }

    private AudioClip GetRandomClip()
    {
        int index = Random.Range(0, _clips.Length);

        return _clips[index];
    }
}
