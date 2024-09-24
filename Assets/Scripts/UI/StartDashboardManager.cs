using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StartDashboardManager : MonoBehaviour
{
    // [SerializeField] private GameManager gameManager;
    
    public GameObject startPanel;

    public GameObject levelPanel;
    public int levelToLoad;
    
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.needLoadGame)
        {
            levelToLoad = GameManager.Instance.levelToLoad;
            LoadLevel();
        }
    }

    public void PlayGame()
    {
        startPanel.SetActive(false);
        levelPanel.SetActive(true);
    }

    public void LoadLevel()
    {
        startPanel.SetActive(false);
        levelPanel.SetActive(true);
        var gridNumber = levelToLoad / 9;
        var levelSelectManager = FindObjectOfType<LevelSelectManager>();
        levelSelectManager.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        var selectedGrid = levelSelectManager.gameObject.transform.GetChild(gridNumber).gameObject;
        selectedGrid.SetActive(true);
        var levelButton = selectedGrid.transform.GetChild(levelToLoad - gridNumber * 9 - 1).gameObject;
        if (levelButton.TryGetComponent(out LevelButton levelButtonComponent))
        {
            levelButtonComponent.ConfirmPanel();
        }
    }

    public void Home()
    {
        // startPanel.SetActive(true);
        // levelPanel.SetActive(false);
    }
}
