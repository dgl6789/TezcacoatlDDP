using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour {

    public static ScoreController Instance;

    [SerializeField] TextMeshPro mainMenuBestText;
    [SerializeField] TextMeshPro GameSceneScoreText;

    int score;
    int highScore;

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    void Start() {
        if(PlayerPrefs.HasKey("highscore")) {
            highScore = PlayerPrefs.GetInt("highscore");
        } else {
            PlayerPrefs.SetInt("highscore", 0);
        }

        UpdateScoreTexts();
    }

    public void UpdateScoreTexts() {
        mainMenuBestText.text = "BEST: " + highScore;
        GameSceneScoreText.text = "SCORE: " + score;
    }

    public void AddScore() {
        score++;

        if(score > highScore) {
            PlayerPrefs.SetInt("highscore", score);
        }

        UpdateScoreTexts();
    }
}
