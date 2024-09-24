using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPanel : MonoBehaviour
{
    private GameData gameData;
    
    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        // for (int i = 0; i < panels.Length; i++)
        // {
        //     panels[i].SetActive(false);
        // }
        //
        // if (gameData != null)
        // {
        //     for (int i = 0; i < gameData.saveData.isActive.Length; i++)
        //     {
        //         if (gameData.saveData.isActive[i])
        //         {
        //             currentLevel = i;
        //         }
        //     }
        // }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
