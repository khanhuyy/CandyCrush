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

    void Start()
    {
        // board = FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        // scoreText.SetText("" + score);
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        if(board != null && scoreBar != null)
        {
            int length = board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score/(float)board.scoreGoals[length - 1];
        }
    }
}
