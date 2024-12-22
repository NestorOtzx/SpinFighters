using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayAgainButton : NetworkBehaviour
{
    public void PlayAgain()
    {
        CallLevelServerRpc();
    }

    [ServerRpc(RequireOwnership =false)]
    private void CallLevelServerRpc()
    {
        GameManager.instance.LoadGameScene(Utilities.SceneNames.MatchMaking.ToString());
    }
}
