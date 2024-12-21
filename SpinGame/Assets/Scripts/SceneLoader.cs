using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;
    private void Awake()
    {
        instance = this;
    }
    public void SetScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void SetNetworkScene(string name)
    {
        GameManager.instance.LoadGameScene(name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
