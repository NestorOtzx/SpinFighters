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
    [SerializeField] private GameObject settingsContainer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (container.activeSelf)
        {
            if (!settingsContainer.activeSelf)
            {
                SetPause(false);    
            }
        }else{
            if (!settingsContainer.activeSelf)
            {
                SetPause(true);
            }
        }
    }


    public void SetPause(bool t)
    {
        SetContainer(t);
        if (GameManager.instance.isSinglePlayer)
        {
            Time.timeScale = t?0:1;
        }
    }

    public void SetContainer(bool t)
    {
        container.SetActive(t);
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
