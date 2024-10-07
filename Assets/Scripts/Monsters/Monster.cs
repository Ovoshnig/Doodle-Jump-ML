using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Monster : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake() => _audioSource = GetComponent<AudioSource>();
}
