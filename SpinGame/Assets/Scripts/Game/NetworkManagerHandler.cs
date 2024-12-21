using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class NetworkManagerHandler : MonoBehaviour
{
    private void Awake()
    {

        // Verificar si ya existe otro NetworkManager activo
        if (NetworkManager.Singleton != null && NetworkManager.Singleton != this.GetComponent<NetworkManager>())
        {
            //Debug.LogWarning("Ya existe un NetworkManager activo. Destruyendo este objeto.");
            Destroy(gameObject);
        }
        else{
            DontDestroyOnLoad(gameObject);
        }
    }
}
