using System;
using Controller;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject backgroundPanel;
    public GameObject victoryPanel;
    public GameObject losePanel;

    public int goal;
    public int moves;
    public int points;

    public bool isGameEnded;
    
    public bool levelSelectPanelIsActive;
    public bool dashboardPanelIsActive;
    public bool confirmPlayPanelIsActive;

    public bool needLoadGame;
    public int levelToLoad;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
    }

    public void Initialize(int _moves, int _goal)
    {
        moves = _moves;
        goal = _goal;
    }
    
    #region "Scene state"
    public void ToSplashHomePanel()
    {
        levelSelectPanelIsActive = false;
        confirmPlayPanelIsActive = false;
        dashboardPanelIsActive = true;
    }
    
    public void ToSplashLevelSelectPanel()
    {
        levelSelectPanelIsActive = true;
        confirmPlayPanelIsActive = false;
        dashboardPanelIsActive = false;
    }
    #endregion

    public void ProcessTurn(int count, bool _substractMoves)
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        
        
    }

    // [RuntimeInitializeOnLoadMethod]
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameEvent.GoToLevel += GameEvent_GoToLevel;
        GameEvent.Retrylevel += GameEvent_RetryLevel;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameEvent.GoToLevel -= GameEvent_GoToLevel;
        GameEvent.Retrylevel -= GameEvent_RetryLevel;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
    
    void GameEvent_GoToLevel(int targetLevel)
    {
        needLoadGame = true;
        levelToLoad = targetLevel;
        Debug.Log("Hello");
    }
    
    void GameEvent_RetryLevel(int targetLevel)
    {
        needLoadGame = true;
        levelToLoad = targetLevel;
    }
}
