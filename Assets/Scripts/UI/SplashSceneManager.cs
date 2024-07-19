using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashSceneManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject dashboardPanel;
    [SerializeField] private GameObject confirmPlayPanel;

    private void OnEnable()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // todo refactor
    private void Update()
    {
        levelSelectPanel.SetActive(gameManager.levelSelectPanelIsActive);
        dashboardPanel.SetActive(gameManager.dashboardPanelIsActive);
        confirmPlayPanel.SetActive(gameManager.confirmPlayPanelIsActive);
    }
}
