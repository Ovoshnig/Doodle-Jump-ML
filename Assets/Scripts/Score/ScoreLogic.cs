using System;
using System.Collections;
using UnityEngine;

public class ScoreLogic : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _multiplier = 1f;
    [SerializeField, Min(0f)] private float _increaseSpeed = 2f;
    [SerializeField] private PlayerMover _playerMover;

    private Coroutine _scoreCoroutine;
    private float _score = 0f;

    public event Action<int> ScoreUpdated;

    private void OnEnable()
    {
        _playerMover.EpisodeBegan += OnEpisodeBegan;
        _playerMover.NewHeightReached += OnNewHeightReached;
    }

    private void OnDisable()
    {
        _playerMover.EpisodeBegan -= OnEpisodeBegan;
        _playerMover.NewHeightReached -= OnNewHeightReached;
    }

    private void OnEpisodeBegan()
    {
        _score = 0f;
        ScoreUpdated?.Invoke((int)_score);
    }

    private void OnNewHeightReached(float newHeight, bool usingBooster)
    {
        float targetScore = newHeight * _multiplier;

        if (_scoreCoroutine != null)
            StopCoroutine(_scoreCoroutine);

        if (usingBooster)
        {
            _score = targetScore;
            ScoreUpdated?.Invoke((int)_score);
        }
        else
        {
            _scoreCoroutine = StartCoroutine(IncreaseScoreSmoothly(targetScore));
        }
    }

    private IEnumerator IncreaseScoreSmoothly(float targetScore)
    {
        while (targetScore - _score > 1f)
        {
            _score = Mathf.Lerp(_score, targetScore, _increaseSpeed * Time.deltaTime);
            ScoreUpdated?.Invoke((int)_score);

            yield return null;
        }

        _score = targetScore;
        ScoreUpdated?.Invoke((int)_score);
    }
}
