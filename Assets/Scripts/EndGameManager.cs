using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameType
{
    Moves, 
    Time
}

[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{
    public GameObject movesLabel, timeLabel;
    public GameObject wonPanel;
    public GameObject tryAgainPanel;
    public TextMeshProUGUI counter;
    public EndGameRequirements requirements;
    public int currentCounterValue;
    private Board board;
    private float timerSeconds;

    void Start()
    {
        // board = FindObjectOfType<Board>();
        // SetupGame();
    }

    void SetupGame()
    {
        currentCounterValue = requirements.counterValue;
        if(requirements.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        } else 
        {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }
        counter.SetText("" + currentCounterValue);
    }

    public void DecreaseCounterValue()
    {
        if(board.currentState != GameState.pause)
        {
            currentCounterValue--;
            counter.SetText("" + currentCounterValue);
            if(currentCounterValue == 0)
            {
                LoseGame();
            }
        }
        // } else {
        //     Debug.Log("You lose");
        // }    
    }

    public void WinGame()
    {
        wonPanel.SetActive(true);
        board.currentState = GameState.win;
        currentCounterValue = 0;
        counter.SetText("" + currentCounterValue);
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        Debug.Log("You lose!");
        currentCounterValue = 0;
        counter.SetText("" + currentCounterValue);
    }

    void Update()
    {
        if(requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if(timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
