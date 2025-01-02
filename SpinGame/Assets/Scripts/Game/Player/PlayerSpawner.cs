using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private Transform [] spawnPoints;
    void Start()
    {
        GameManager.instance.SetSpawns(spawnPoints);
        if (GameManager.instance.isSinglePlayer)
        {
            GameManager.instance.SpawnSinglePlayer();
        }else{
            if (NetworkManager.Singleton.IsServer)
            {
                for  (int i = 0; i< NetworkManager.Singleton.ConnectedClientsIds.Count; i++)
                {
                    GameManager.instance.SpawnPlayerSrv(NetworkManager.Singleton.ConnectedClientsIds[i], i);
                }
            }

        }
    }
}
