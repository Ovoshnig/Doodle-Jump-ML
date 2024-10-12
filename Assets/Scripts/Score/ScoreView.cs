using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ScoreView : MonoBehaviour
{
    [SerializeField] private ScoreLogic _logic;

    private TMP_Text _text;

    private void Awake() => _text = GetComponent<TMP_Text>();

    private void OnEnable() => _logic.ScoreUpdated += OnScoreUpdated;

    private void OnDisable() => _logic.ScoreUpdated -= OnScoreUpdated;

    private void OnScoreUpdated(int score) => _text.text = "Score: " + score;
}
