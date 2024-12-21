using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform [] spawnPoints;
    void Start()
    {
        GameManager.instance.SetSpawns(spawnPoints);
        if (IsOwner)
        {
            for  (int i = 0; i< NetworkManager.Singleton.ConnectedClientsIds.Count; i++)
            {
                GameManager.instance.SpawnPlayerServerRpc(NetworkManager.Singleton.ConnectedClientsIds[i], i);
            }
        }
    }
}
