using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class PlayerInfo : NetworkBehaviour
{
    public ulong playerID;

    private void Start()
    {
        if (GameManager.instance.isSinglePlayer)
        {
            playerID = (ulong)GameManager.instance.remainingPlayersSingle.Count;
            gameObject.name = "Player "+(playerID).ToString();
            GameManager.instance.OnSpawnNewPlayer(this);
        }
    }

    public override void OnNetworkSpawn()
    {
        playerID = OwnerClientId;
        GameManager.instance.OnSpawnNewPlayer(this);
        gameObject.name = "Player "+(playerID).ToString();
        Debug.Log("[Debug All] Player " +gameObject.name + " Connected!");
    }


}
