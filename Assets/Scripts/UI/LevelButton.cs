using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("Active Stuff")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    private Image buttonImage;
    private Button button;
    private int starsActive;

    [Header("Level UI")]
    public Image starsContainer;
    public Image[] stars;
    public TextMeshProUGUI levelText;
    public int level;
    public GameObject confirmPanel;
    
    [Header("Manager")]
    private GameManager gameManager;
    private GameData gameData;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameData = FindObjectOfType<GameData>();
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();
        LoadData();
        ActivateStars();
        ShowLevel();
        DecideSprite();
    }

    void LoadData()
    {
        if (gameData != null)
        {
            // level is unlocked
            if (gameData.saveData.isActive[level - 1])
            {
                isActive = true;
            }
            else
            {
                isActive = false;
            }
            // stars
            starsActive = gameData.saveData.stars[level - 1];
        }
    }
    
    void ActivateStars()
    {
        for (int i = 0; i < starsActive; i++)
        { 
            stars[i].gameObject.SetActive(enabled = true);
        }
    }

    void DecideSprite()
    {
        if (isActive)
        {
            buttonImage.sprite = activeSprite;
            button.enabled = true;
            levelText.enabled = true;
            starsContainer.enabled = true;
        }
        else
        {
            buttonImage.sprite = lockedSprite;
            button.enabled = false;
            levelText.enabled = false;
            starsContainer.enabled = false;
        }
    }

    void ShowLevel()
    {
        levelText.text = "" + level;
    }

    public void ConfirmPanel()
    {
        if (confirmPanel.TryGetComponent(out ConfirmPanel script))
        {
            script.level = level;
        }

        gameManager.confirmPlayPanelIsActive = true;
        // confirmPanel.SetActive(true);
    }
}
