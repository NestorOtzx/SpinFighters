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

    public GameObject playerPrefab;

    private Transform [] playerSpawns;

  


    private void Awake()
    {
        remainingPlayerIDs = new NetworkList<ulong>();
        if (instance == null)
        {
            instance = this;
            
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("Ya existe GM");
            if (instance.IsServer)
            {
                Debug.Log("Reinicia las variables!!!");
                instance.remainingPlayerIDs.Clear();
            }
            instance.players.Clear();           
            Destroy(gameObject);
        }
    }

    public void SetSpawns(Transform [] spawns)
    {
        playerSpawns = spawns;
    }

    [ServerRpc]
    public void SpawnPlayerServerRpc(ulong clientId, int spawn_id)
    {
        // Instanciar el jugador
        GameObject playerInstance = Instantiate(playerPrefab, playerSpawns[spawn_id].position, Quaternion.identity);
        // Asignar la propiedad NetworkObject al cliente especificado
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    

#region NETWORK_EVENTS

    private void OnEnable()
    {
        // Suscribirse al evento que se llama cuando se carga una escena
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStopped += OnServerStopped;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }
    }

    private void OnDisable()
    {
        // Desuscribirse para evitar errores al destruir el objeto
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
        }
    }

    private void OnClientDisconnect(ulong clientID){
        if (IsOwner)
        {
            Debug.Log("Cliente "+ clientID + " desconectado.");
            SetLooserServerRpc(clientID);
        }
    }

    private void OnServerStopped(bool action){
        Debug.Log("Servidor cerrado " + action);
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

    [ServerRpc(RequireOwnership = false)]
    private void DisconnectClientServerRpc(ulong clientId)
    {
        
        NetworkManager.Singleton.DisconnectClient(clientId);
        if (NetworkManager.Singleton.ConnectedClientsIds.Count == 0)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            
            Debug.Log($"Host {clientId} connected to the server.");
        }
        else
        {
            Debug.Log($"Client {clientId} connected to the server.");
        }
    }

#endregion

    

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

        if (IsServer)
        {
            remainingPlayerIDs.Add(player.OwnerClientId);
        }
    }    

    [ServerRpc]
    public void SetLooserServerRpc(ulong looserID)
    {
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
        OnEndRoundClientRpc(remainingPlayerIDs[0]);
        StartCoroutine(NextRound());
    }

    [ClientRpc]
    private void OnEndRoundClientRpc(ulong winner)
    {
        Debug.Log("[Client] end round, i am client: "+clientPlayerID+" and winner is "+ winner);
        if (winner == clientPlayerID)
        {
            UIManager.instance.SetWin(true);
        }else{
            UIManager.instance.SetLoose(true);
        }
        
    }

    IEnumerator NextRound()
    {
        if (instance)
        {
            yield return new WaitForSeconds(1);
            
            
            var playerCopy = new List<NetworkPlayerInfo>(players.Values); 

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
        roundsPlayed.Value = 0;

    }
}
