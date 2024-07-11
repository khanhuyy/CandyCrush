using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private Board board;
    public TextMeshProUGUI scoreText;
    public int score;
    public Image scoreBar;
    private GameData gameData;
    
    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();
        UpdateBar();
    }

    void Update()
    {
        scoreText.SetText("" + score);
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        if (gameData != null)
        {
            int highScore = gameData.saveData.highScroes[board.level];
            if (score > highScore)
            {
                gameData.saveData.highScroes[board.level] = score;

            }
            gameData.Save();
        }
        UpdateBar();
    }

    private void UpdateBar()
    {
        if(board != null && scoreBar != null)
        {
            int length = board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score/(float)board.scoreGoals[length - 1];
        } 
    }
}
