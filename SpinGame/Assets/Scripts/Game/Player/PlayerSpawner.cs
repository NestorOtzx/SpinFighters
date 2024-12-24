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
        if (NetworkManager.Singleton.IsServer)
        {
            for  (int i = 0; i< NetworkManager.Singleton.ConnectedClientsIds.Count; i++)
            {
                GameManager.instance.SpawnPlayerSrv(NetworkManager.Singleton.ConnectedClientsIds[i], i);
            }
        }
    }
}
