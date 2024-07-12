using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pausePanel;
    private Board board;
    public bool paused = false;
    [FormerlySerializedAs("soundButtonSprite")] public Image soundButton;
    
    // todo make animation
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    private SoundManager sound;
    
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = musicOffSprite;
            }
            else
            {
                soundButton.sprite = musicOnSprite;
            }
        }
        else
        {
            soundButton.sprite = musicOnSprite;
        }
        pausePanel.SetActive(false);
        board = FindObjectOfType<Board>();
        sound = FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (paused && !pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(true);
            board.currentState = GameState.Pause;
        }

        if (!paused && pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            board.currentState = GameState.Move;
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
                sound.AdjustVolumn();
            }
            else
            {
                soundButton.sprite = musicOffSprite;
                PlayerPrefs.SetInt("Sound", 0);
                sound.AdjustVolumn();
            }
        }
        else
        {
            soundButton.sprite = musicOnSprite;
            PlayerPrefs.SetInt("Sound", 1);
            sound.AdjustVolumn();
        }
    }
    
    public void PauseOrContinueGame()
    {
        paused = !paused;
    }

    public void ToMainMenu()
    {
        
    }
}
