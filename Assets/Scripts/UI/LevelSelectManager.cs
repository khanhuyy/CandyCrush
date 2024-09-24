using TMPro;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    public World world;
    public GameObject[] panels;
    public GameObject currentPanel;
    public int page;
    private GameData gameData;
    public int currentLevel = 0;
    public TextMeshProUGUI paginationText;
    private const int LevelPerPage = 9;
    public GameObject goalsContainer;
    
    // Start is called before the first frame update
    void Start()
    {
        if (world)
        {
            
        }
        gameData = FindObjectOfType<GameData>();
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }

        if (gameData != null)
        {
            for (int i = 0; i < gameData.saveData.isActive.Length; i++)
            {
                if (gameData.saveData.isActive[i])
                {
                    currentLevel = i;
                }
            }
        }

        page = Mathf.FloorToInt(currentLevel / LevelPerPage);
        UpdatePaginationText();
        currentPanel = panels[page];
        panels[page].SetActive(true);
    }

    public void PageRight()
    {
        if (page < panels.Length - 1)
        {
            currentPanel.SetActive(false);
            page++;
            currentPanel = panels[page];
            currentPanel.SetActive(true);
            UpdatePaginationText();
        }
    }
    
    public void PageLeft()
    {
        if (page > 0)
        {
            currentPanel.SetActive(false);
            page--;
            currentPanel = panels[page];
            currentPanel.SetActive(true);
            UpdatePaginationText();
        }
    }

    private void UpdatePaginationText()
    {
        paginationText.text = (page + 1) + "/" + panels.Length;
    }

    public void CloseConfirmPanel()
    {
        foreach (Transform goal in goalsContainer.transform)
        {
            Destroy(goal.gameObject);
        }
    }
}
