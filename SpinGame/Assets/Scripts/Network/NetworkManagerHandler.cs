using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerHandler : MonoBehaviour
{
    private void Awake()
    {

        // Verificar si ya existe otro NetworkManager activo
        if (NetworkManager.Singleton != null && NetworkManager.Singleton != this.GetComponent<NetworkManager>() )
        {
            if (SceneManager.GetActiveScene().name == Utilities.SceneNames.JoinMatch.ToString())
            {
                NetworkManager.Singleton.Shutdown();
            }
            Destroy(gameObject);
        }else{
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == Utilities.SceneNames.MainMenu.ToString()) //en el main menu no habrá sesiones
        {
            Destroy(gameObject);
        }
    }
}
