using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class NetworkPlayerInfo : NetworkBehaviour
{

    private void Start()
    {
        GameManager.instance.OnSpawnNewPlayer(this);
        gameObject.name = "Player "+(OwnerClientId).ToString();
        Debug.Log("[Debug All] Player " +gameObject.name + " Connected!");
    }
}
