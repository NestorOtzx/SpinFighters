using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerHandler : MonoBehaviour
{
    private void Awake()
    {

        // Verificar si ya existe otro NetworkManager activo
        if (NetworkManager.Singleton != null && NetworkManager.Singleton != this.GetComponent<NetworkManager>() )
        {
            Destroy(gameObject);
        }
        else if (SceneManager.GetActiveScene().name == Utilities.SceneNames.MainMenu.ToString()) //en el main menu no habrá sesiones
        {
            Destroy(gameObject);
        }
        else{
            DontDestroyOnLoad(gameObject);
        }
    }
}
