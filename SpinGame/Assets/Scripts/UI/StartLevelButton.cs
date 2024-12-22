using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLevelButton : NetworkBehaviour
{
    [SerializeField] private string scene;
    [SerializeField] private int minimumPlayers = 2;

    public void TryStartGame()
    {
        Debug.Log("Boton presionado!!!");
        CallLevelServerRpc();
        
    }

    [ServerRpc(RequireOwnership =false)]
    public void CallLevelServerRpc()
    {
        Debug.Log("Server RPC 1 clients: " + NetworkManager.Singleton.ConnectedClientsIds.Count);
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= minimumPlayers)
        {
            Debug.Log("Server RPC 2");
            NetworkManager.Singleton.SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }else{
            //handle error
        }
    }


}
