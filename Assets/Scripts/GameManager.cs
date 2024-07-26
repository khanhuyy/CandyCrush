using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public bool levelSelectPanelIsActive;
    public bool dashboardPanelIsActive;
    public bool confirmPlayPanelIsActive;
    
    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
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
}
