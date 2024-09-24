using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ConfirmPanel : MonoBehaviour
{
    [FormerlySerializedAs("levelToLoad")] [Header("Level Information")]
    public string sceneToLoad;
    private GameData gameData;
    private int starsActive;
    private int highScore;

    [Header("UI stuff")]
    public TextMeshProUGUI levelTitle;
    public Image[] stars;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI starText;
    public TextMeshProUGUI targetScoreAccordingStar;
    public Level level;
    public GameObject goalsContainer;
    public GameObject goalPrefab;
    
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
        foreach (var goal in level.levelGoals)
        {
            GameObject createdGoal = Instantiate(goalPrefab, goalsContainer.transform.position, Quaternion.identity, goalsContainer.transform);
            if (createdGoal.TryGetComponent(out Image goalImage))
            {
                goalImage.sprite = goal.goalSprite;
            }

            createdGoal.GetComponentInChildren<TextMeshProUGUI>().text = goal.numberNeeded.ToString();
        }
    }

    public void SetLevel(Level worldLevel)
    {
        level = worldLevel;
    }

    void LoadData()
    {
        Debug.Log(level.number);
        if (gameData != null)
        {
            starsActive = gameData.saveData.stars[level.number];
            highScore = gameData.saveData.highScores[level.number];
        }
    }

    void SetText()
    {
        levelTitle.text = "Level " + level.number;
        highScoreText.text = highScore.ToString();
        starText.text = "x" + (starsActive == 3 ? starsActive : starsActive + 1);
        targetScoreAccordingStar.text = "Target: " + level.scoreGoals[starsActive == 3 ? starsActive - 1 : starsActive];
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
        PlayerPrefs.SetInt("Current Level", level.number); // todo refactor sync
        SceneManager.LoadScene(sceneToLoad);
    }
}
