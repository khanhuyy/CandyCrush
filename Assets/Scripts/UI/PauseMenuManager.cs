using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pausePanel;
    private Board _board;
    public bool paused;
    public Image soundButton;
    
    // todo make animation
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    private SoundManager _sound;
    private Animator _pausePanelAnimator;

    private GameManager _gameManager;
    private static readonly int Close = Animator.StringToHash("Close");

    private void Start()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            soundButton.sprite = PlayerPrefs.GetInt("Sound") == 0 ? musicOffSprite : musicOnSprite;
        }
        else
        {
            soundButton.sprite = musicOnSprite;
        }

        _gameManager = FindFirstObjectByType<GameManager>();
        _pausePanelAnimator = pausePanel.GetComponentInChildren<Animator>();
        pausePanel.SetActive(false);
        _board = FindFirstObjectByType<Board>();
        _sound = FindFirstObjectByType<SoundManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (paused)
        {
            case true when !pausePanel.activeInHierarchy:
                pausePanel.SetActive(true);
                _board.currentState = GameState.Pause;
                break;
            case false when pausePanel.activeInHierarchy:
                pausePanel.SetActive(false);
                _board.currentState = GameState.Move;
                _pausePanelAnimator.SetTrigger(Close);
                break;
        }
    }
    
    public void SoundButton()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = musicOnSprite;
                PlayerPrefs.SetInt("Sound", 1);
                _sound.AdjustVolume();
            }
            else
            {
                soundButton.sprite = musicOffSprite;
                PlayerPrefs.SetInt("Sound", 0);
                _sound.AdjustVolume();
            }
        }
        else
        {
            soundButton.sprite = musicOnSprite;
            PlayerPrefs.SetInt("Sound", 1);
            _sound.AdjustVolume();
        }
    }
    
    public void PauseOrContinueGame()
    {
        paused = !paused;
    }

    public void ToMainMenu()
    {
        _gameManager.dashboardPanelIsActive = true;
        _gameManager.confirmPlayPanelIsActive = false;
        _gameManager.levelSelectPanelIsActive = false;
        SceneManager.LoadScene("Scenes/Splash");
    }
}
