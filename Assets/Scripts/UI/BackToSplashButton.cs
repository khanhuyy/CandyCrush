using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackToSplashButton : MonoBehaviour
{
    public string sceneToLoad;

    public void OK()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
