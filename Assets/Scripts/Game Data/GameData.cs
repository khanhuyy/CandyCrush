using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Serialization;

[Serializable]
public enum PowerUp
{
    Lightning,
    Rainbow,
    Doom
}

[Serializable]
public class PowerUpData
{
    public PowerUp type;
    public int quantity;
}

[Serializable]
public class SaveData
{
    public bool[] isActive;
    public int[] highScores;
    public int[] stars;
    public PowerUpData[] powerUps;
}

public class GameData : MonoBehaviour
{
    public static GameData instance;
    public SaveData saveData;
    
    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        // else
        // {
        //     Destroy(this.gameObject);
        // }
        Load();
    }

    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
        SaveData data = new SaveData();
        data = saveData;
        formatter.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/player.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
        }
        else
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Create);
            saveData = new SaveData();
            saveData.isActive = new bool[100];
            saveData.stars = new int[100];
            saveData.highScores = new int[100];
            saveData.isActive[0] = true;
            saveData.powerUps = new PowerUpData[3];
            saveData.powerUps[0] = new PowerUpData
            {
                type = PowerUp.Lightning,
                quantity = 0
            };
            saveData.powerUps[1] = new PowerUpData
            {
                type = PowerUp.Rainbow,
                quantity = 0
            };
            saveData.powerUps[2] = new PowerUpData
            {
                type = PowerUp.Doom,
                quantity = 0
            };
            formatter.Serialize(file, saveData);
            file.Close();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnDisable()
    {
        Save();
    }
}
