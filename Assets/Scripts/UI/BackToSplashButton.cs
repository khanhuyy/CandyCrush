using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// todo rename this class
public class BackToSplashButton : MonoBehaviour
{
    public string sceneToLoad;
    private GameManager gameManager;
    private GameData gameData;
    private Board board;
    
    public void WinOK()
    {
        if (gameData != null)
        {
            gameData.saveData.isActive[board.level + 1] = true;
            gameData.Save();
        }
        gameManager.ToSplashLevelSelectPanel();
        SceneManager.LoadScene(sceneToLoad);
    }
    
    public void LoseOK()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void OnHomeClick()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        board = FindObjectOfType<Board>();
        gameManager = FindObjectOfType<GameManager>();
    }
}
