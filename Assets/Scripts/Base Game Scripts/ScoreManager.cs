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
