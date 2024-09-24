using Controller;
using Unity.VisualScripting;
using UnityEngine;

public class SplashSceneManager : MonoBehaviour
{
    public GameObject startPanel;

    public GameObject levelPanel;
    public LevelSelectManager levelSelectManager;
    public ConfirmPanel confirmPanel;
    
    private void OnEnable()
    {
        GameEvent.GoToLevel += GameEvent_GoToLevel;
        GameEvent.Retrylevel += GameEvent_RetryLevel;
    }
    
    void Start()
    {
        startPanel.SetActive(true);
        levelPanel.SetActive(false);
    }
    
    private void OnDisable()
    {
        GameEvent.GoToLevel -= GameEvent_GoToLevel;
        GameEvent.Retrylevel -= GameEvent_RetryLevel;
    }

    private void GameEvent_GoToLevel(int desLevel)
    {
        {
            var gridNumber = desLevel / 9;
            levelSelectManager.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            var selectedGrid = levelSelectManager.gameObject.transform.GetChild(gridNumber).gameObject;
            selectedGrid.SetActive(true);
            var levelButton = selectedGrid.transform.GetChild(desLevel - gridNumber * 9 - 1).gameObject;
            if (levelButton.TryGetComponent(out LevelButton levelButtonComponent))
            {
                confirmPanel.SetLevel(levelButtonComponent.worldLevel);
            }
        }
    }
    
    private void GameEvent_RetryLevel(int levelRetry)
    {

        var gridNumber = levelRetry / 9;
        levelSelectManager.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        var selectedGrid = levelSelectManager.gameObject.transform.GetChild(gridNumber).gameObject;
        selectedGrid.SetActive(true);
        var levelButton = selectedGrid.transform.GetChild(levelRetry - gridNumber * 9 - 1).gameObject;
        if (levelButton.TryGetComponent(out LevelButton levelButtonComponent))
        {
            confirmPanel.SetLevel(levelButtonComponent.worldLevel);
        }
    }

    public void PlayGame()
    {
        startPanel.SetActive(false);
        levelPanel.SetActive(true);
    }

    public void Home()
    {
        startPanel.SetActive(true);
        levelPanel.SetActive(false);
    }
}
