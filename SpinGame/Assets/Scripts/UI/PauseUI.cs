using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject container;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        SetPause(!container.activeSelf);
    }


    public void SetPause(bool t)
    {
        container.SetActive(t);
        if (GameManager.instance.isSinglePlayer)
        {
            Time.timeScale = t?0:1;
        }
    }

    private void OnDestroy()
    {
        SetPause(false);
    }

    public void Exit()
    {
        if (GameManager.instance.isSinglePlayer)
        {
            SceneManager.LoadScene(Utilities.SceneNames.MainMenu.ToString());
        }else{
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(Utilities.SceneNames.MainMenu.ToString());
        }
    }
}
