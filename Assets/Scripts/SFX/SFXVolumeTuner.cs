using UnityEngine;

public class SFXVolumeTuner : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float _volume = 1f;

    private void Update()
    {
        if (AudioListener.volume != _volume)
            AudioListener.volume = _volume;
    }
}
