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
            GameManager.instance.OnSpawnNewPlayer(this);
        }
    }

    public void SetId(ulong id) //For single player only
    {
        Debug.Log("Setting id!!!"+id);
        playerID = id;
        gameObject.name = "Player "+(playerID).ToString();
    }

    public override void OnNetworkSpawn()
    {
        playerID = OwnerClientId;
        GameManager.instance.OnSpawnNewPlayer(this);
        gameObject.name = "Player "+(playerID).ToString();
        Debug.Log("[Debug All] Player " +gameObject.name + " Connected!");
    }


}
