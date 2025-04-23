using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int score { get; private set; }
    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    private int bestScore;

    void Awake()
    {
        bestScore = PlayerPrefs.GetInt("bestScore", 0);
        ResetScore();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetBestScore();
            ResetScore();
        }
    }

    public void ResetScore()
    {
        score = 0;
        PlayerPrefs.SetInt("score", score);
        UpdateScoreUI();
    }

    public void AddScore(int points)
    {
        score += points;
        PlayerPrefs.SetInt("score", score);
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("bestScore", bestScore);
        }
        UpdateScoreUI();
    }

    public void GameOver()
    {
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("bestScore", bestScore);
        }
        ResetScore();
    }

    public void ResetBestScore()
    {
        bestScore = 0;
        PlayerPrefs.SetInt("bestScore", bestScore);
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
        if (bestScoreText != null)
            bestScoreText.text = "Meilleur score: " + bestScore.ToString();
    }
}