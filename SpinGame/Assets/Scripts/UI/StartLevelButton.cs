using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLevelButton : NetworkBehaviour
{
    [SerializeField] private string scene;
    [SerializeField] private int minimumPlayers = 2;

    public void SetScene(string scene)
    {
        this.scene = scene;
    }

    public void TryStartGame()
    {
        Debug.Log("Boton presionado!!!");
        if (GameManager.instance.isSinglePlayer && PlayerConnection.instance.clientInfoSingle.Count > 1)
        {
            SceneManager.LoadScene(scene);
        }else{
            CallLevelServerRpc(scene);
        }
    }

    [ServerRpc(RequireOwnership =false)]
    public void CallLevelServerRpc(string requestScene)
    {
        Debug.Log("Server RPC 1 clients: " + NetworkManager.Singleton.ConnectedClientsIds.Count);
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= minimumPlayers)
        {
            Debug.Log("Server RPC 2");
            NetworkManager.Singleton.SceneManager.LoadScene(requestScene, LoadSceneMode.Single);
        }else{
            //handle error
        }
    }


}
