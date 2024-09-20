using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour
{
    [Header("Level Information")]
    public string levelToLoad;
    public int level;
    private GameData gameData;
    private int starsActive;
    private int highScore;

    [Header("UI stuff")]
    public TextMeshProUGUI levelTitle;
    public Image[] stars;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI starText;
    public TextMeshProUGUI targetScoreAccordingStar;
    public GameObject goals;
    
    void OnEnable()
    {
        gameData = FindObjectOfType<GameData>();
        LoadData();
        ActivateStars();
        SetText();
        SetGoals();
    }

    private void SetGoals()
    {
        
    }

    void LoadData()
    {
        if (gameData != null)
        {
            starsActive = gameData.saveData.stars[level - 1];
            highScore = gameData.saveData.highScores[level - 1];
        }
    }

    void SetText()
    {
        levelTitle.text = "Level " + level;
        highScoreText.text = highScore.ToString();
        starText.text = "x" + (starsActive == 3 ? starsActive : starsActive + 1);
        // targetScoreAccordingStar = "Target: " + g;
    }
    
    void ActivateStars()
    {
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].gameObject.SetActive(true);
        }
    }

    public void Cancel()
    {
        this.gameObject.SetActive(false);
    }

    public void Play()
    {
        PlayerPrefs.SetInt("Current Level", level - 1); // todo refactor sync
        SceneManager.LoadScene(levelToLoad);
    }
}
