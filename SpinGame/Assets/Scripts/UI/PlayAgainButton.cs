using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAgainButton : NetworkBehaviour
{
    public void PlayAgain()
    {
        CallLevelServerRpc();
    }

    [ServerRpc(RequireOwnership =false)]
    private void CallLevelServerRpc()
    {
        GameManager.instance.NetworkObject.Despawn(true);
        GameManager.instance = null;
        NetworkManager.Singleton.SceneManager.LoadScene(Utilities.SceneNames.MatchMaking.ToString(), LoadSceneMode.Single);
    }
}
