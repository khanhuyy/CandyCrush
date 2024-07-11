using System.Collections;
using System.Collections.Generic;
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
    
    
    public Image[] stars;
    public TextMeshProUGUI levelText;
    public int level;
    public GameObject confirmPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();
        ActivateStars();
        ShowLevel();
        DecideSprite();
    }

    void ActivateStars()
    {
        for (int i = 0; i < stars.Length; i++)
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
        }
        else
        {
            buttonImage.sprite = lockedSprite;
            button.enabled = false;
            levelText.enabled = false;
        }
    }

    void ShowLevel()
    {
        levelText.text = "" + level;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConfirmPanel()
    {
        confirmPanel.SetActive(true);
        if (confirmPanel.TryGetComponent(out ConfirmPanel script))
        {
            script.level = level;
        }
    }
}
