using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour
{
    public string levelToLoad;
    public Image[] stars;
    private int starsActive;
    public int level;
    private GameData gameData;

    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        LoadData();
        ActivateStars();
    }

    void LoadData()
    {
        if (gameData != null)
        {
            starsActive = gameData.saveData.stars[level - 1];
        }
    }
    
    void ActivateStars()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(enabled = true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
