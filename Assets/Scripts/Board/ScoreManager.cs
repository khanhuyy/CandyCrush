using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public List<GameObject> stars;
    
    private Board board;
    public TextMeshProUGUI scoreText;
    public int score;
    public Image scoreBar;
    private GameData gameData;
    private int numberStars;
    
    
    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();
        UpdateBar();
    }

    void Update()
    {
        scoreText.SetText("" + score);
        if (score > board.scoreGoals[2])
        {
            stars[2].SetActive(true);
        }
        else if (score > board.scoreGoals[1])
        {
            stars[1].SetActive(true);
        }
        else if (score > board.scoreGoals[0])
        {
            stars[0].SetActive(true);
        }
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        for (int i = 0; i < board.scoreGoals.Length; i++)
        {
            if (score > board.scoreGoals[i] && numberStars < i + 1)
            {
                numberStars++;
            }
        }
        if (gameData)
        {
            int highScore = gameData.saveData.highScores[board.level];
            if (score > highScore)
            {
                gameData.saveData.highScores[board.level] = score;
            }

            int currentStars = gameData.saveData.stars[board.level];
            if (numberStars > currentStars)
            {
                gameData.saveData.stars[board.level] = numberStars;
            }
            gameData.Save();
        }
        UpdateBar();
    }

    private void UpdateBar()
    {
        if(board && scoreBar)
        {
            int length = board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score/board.scoreGoals[length - 1];
        } 
    }
}
