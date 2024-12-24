using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public Dictionary<ulong, NetworkPlayerInfo> players = new Dictionary<ulong, NetworkPlayerInfo>();
    public NetworkList<ulong> remainingPlayerIDs;
    public int numberOfMatches = 3;
    private NetworkVariable<int> roundsPlayed = new NetworkVariable<int>(0);

    public ulong clientPlayerID = 666666;

    [SerializeField] private GameObject playerPrefab;

    private Transform [] playerSpawns;

    private void Awake()
    {
        remainingPlayerIDs = new NetworkList<ulong>();
        if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetSpawns(Transform [] spawns)
    {
        playerSpawns = spawns;
    }

    public void SpawnPlayerSrv(ulong clientId, int spawn_id)
    {
        //Instanciar el jugador
        GameObject playerInstance = Instantiate(playerPrefab, playerSpawns[spawn_id].position, Quaternion.identity);
        // Asignar la propiedad NetworkObject al cliente especificado
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
    private void OnEnable()
    {
        // Suscribirse al evento que se llama cuando se carga una escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Desuscribirse para evitar errores al destruir el objeto
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.ToLower() == "mainmenu")
        {
            Debug.Log("Loaded main menu, instance: "+instance.gameObject.name);
            if (NetworkManager.Singleton && instance)
            {
                Debug.Log("Network: " + NetworkManager.Singleton);
                if (NetworkManager.Singleton.IsClient)
                {
                    Debug.Log("Disconnect client "+instance.OwnerClientId + " current clients: "+ NetworkManager.Singleton.ConnectedClientsIds.Count);
                }else{
                    Debug.Log("Disconnect host "+instance.OwnerClientId + " current clients: "+ NetworkManager.Singleton.ConnectedClientsIds.Count);
                }
                
                NetworkManager.Singleton.Shutdown();
            }
            Destroy(gameObject);
        }
    }   
    

    public void OnSpawnNewPlayer(NetworkPlayerInfo player)
    {
        Debug.Log("Spawn player, is owner: "+player.IsOwner+", current players:");
        foreach(var p in remainingPlayerIDs)
        {
            Debug.Log("[Server] Remaining player: "+p);
        }
        players.Add(player.OwnerClientId, player);
        if (player.IsOwner)
        {
            clientPlayerID = player.OwnerClientId;
        }

        Debug.Log("Spawn player, is server: "+NetworkManager.Singleton.IsServer+", list:"+remainingPlayerIDs);
        if (NetworkManager.Singleton.IsServer)
        {
            remainingPlayerIDs.Add(player.OwnerClientId);
        }
    }    

    public void SetLooserSrv(ulong looserID)
    {
        Debug.Log("Set looser "+looserID);
        foreach (var el in remainingPlayerIDs)
        {
            Debug.Log("El: "+el);
        }
        if (remainingPlayerIDs.Contains(looserID))
        {
            Debug.Log("[DebugServer] Player " + players[looserID].name + " lost the game!");
            remainingPlayerIDs.Remove(looserID);
            if (remainingPlayerIDs.Count == 1)
            {
                OnEndRound();
            }
        }
    }


    public override void OnDestroy()
    {
        remainingPlayerIDs.Dispose(); // Limpia correctamente si ya no se usa.
    }

    private void OnEndRound()
    {
        roundsPlayed.Value+=1;
        Debug.Log("[Server] End round "+ (roundsPlayed.Value));
        foreach(var p in remainingPlayerIDs)
        {
            Debug.Log("[Server] Remaining player: "+p);
        }
        List<NetworkPlayerInfo> playerCopy = new List<NetworkPlayerInfo>(players.Values);
        OnEndRoundClientRpc(remainingPlayerIDs[0]);
        remainingPlayerIDs.Clear();
        players.Clear();
        StartCoroutine(NextRound(playerCopy));
    }

    [ClientRpc]
    private void OnEndRoundClientRpc(ulong winner)
    {
        players.Clear();
        Debug.Log("[Client] end round, i am client: "+clientPlayerID+" and winner is "+ winner);
        if (winner == clientPlayerID)
        {
            UIManager.instance.SetWin(true);
        }else{
            UIManager.instance.SetLoose(true);
        }
        
    }

    IEnumerator NextRound(List<NetworkPlayerInfo> playerCopy)
    {
        if (instance)
        {
            yield return new WaitForSeconds(1);
            
            
            

            if (instance.roundsPlayed.Value < instance.numberOfMatches && NetworkManager.Singleton.ConnectedClientsIds.Count > 1)
            {
                instance.LoadGameScene(SceneManager.GetActiveScene().name);
            }else{
                instance.LoadGameScene(Utilities.SceneNames.ScoresScreen.ToString());
            }


            foreach (var p in playerCopy)
            {
                if (p != null)
                {
                    Destroy(p.gameObject);
                }
            }
            
        }
    }

    public void LoadGameScene(string name)
    {
        //SceneManager.LoadScene(sceneIndex);        
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(name, LoadSceneMode.Single);
        }
    }

    public void RestartMatch()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            roundsPlayed.Value = 0;
        }
    }
}
