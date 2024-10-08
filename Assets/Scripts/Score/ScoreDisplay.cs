using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ScoreDisplay : MonoBehaviour
{
    [SerializeField, Min(0)] private float _multiplier = 1;
    [SerializeField] private PlayerMover _playerMover;

    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();

        _playerMover.NewHeightReached += OnNewPlatformReached;
    }

    private void OnDestroy() => _playerMover.NewHeightReached -= OnNewPlatformReached;

    private void OnNewPlatformReached(float newHeight) => _text.text = "Score: " + (int)(newHeight * _multiplier);
}
