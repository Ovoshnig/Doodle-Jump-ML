using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ScoreDisplay : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _multiplier = 1f;
    [SerializeField, Min(0f)] private float _increaseSpeed = 2f;
    [SerializeField] private PlayerMover _playerMover;

    private TMP_Text _text;
    private Coroutine _scoreCoroutine;
    private float _score = 0f;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();

        _playerMover.NewHeightReached += OnNewPlatformReached;
    }

    private void Start() => Display();

    private void OnDestroy() => _playerMover.NewHeightReached -= OnNewPlatformReached;

    private void OnNewPlatformReached(float newHeight)
    {
        float score = newHeight * _multiplier;

        if (_scoreCoroutine != null)
            StopCoroutine(_scoreCoroutine);

        _scoreCoroutine = StartCoroutine(IncreaseScoreSmoothly(score));
    }

    private IEnumerator IncreaseScoreSmoothly(float targetScore)
    {
        while (targetScore - _score > 1f)
        {
            _score = Mathf.Lerp(_score, targetScore, _increaseSpeed * Time.deltaTime);
            Display();

            yield return null;
        }

        _score = targetScore;
        Display();
    }

    private void Display() => _text.text = "Score: " + (int)(_score);
}
