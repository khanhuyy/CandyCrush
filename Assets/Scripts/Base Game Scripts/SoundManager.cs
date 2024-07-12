using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoise;
    public AudioSource backgroundMusic;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 1)
            {
                backgroundMusic.Play();
                backgroundMusic.volume = 0;
            }
            else
            {
                backgroundMusic.Play();
                backgroundMusic.volume = 1;
            }
        }
        else
        {
            backgroundMusic.Play();
            backgroundMusic.volume = 1;
        }
    }

    public void AdjustVolumn()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                backgroundMusic.volume = 0;
            }
            else
            {
                backgroundMusic.volume = 1;
            }
        }
    }
    
    public void PlayRandomDestroyNoise()
    {
        if(destroyNoise.Length > 0)
        {
            if (PlayerPrefs.HasKey("Sound"))
            {
                if (PlayerPrefs.GetInt("Sound") == 1)
                {
                    int clipToPlay = Random.Range(0, destroyNoise.Length);
                    destroyNoise[clipToPlay].Play();    
                }
            }
        }
        else
        {
            int clipToPlay = Random.Range(0, destroyNoise.Length);
            destroyNoise[clipToPlay].Play(); 
        }
    }
}
