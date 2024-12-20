using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public List<NetworkPlayerInfo> players = new List<NetworkPlayerInfo>();
    public NetworkList<int> remainingPlayerIDs;

    

    public int numberOfMatches = 3;
    private NetworkVariable<int> matchesPlayed = new NetworkVariable<int>(0);

    public int clientPlayerID = -1;

    public GameObject playerPrefab;

    private Transform [] playerSpawns;

  


    private void Awake()
    {
        remainingPlayerIDs = new NetworkList<int>();
        if (instance == null)
        {
            instance = this;
            
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
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

    

#region GM_SCENE_LOADING


    private void OnEnable()
    {
        // Suscribirse al evento que se llama cuando se carga una escena
        SceneManager.sceneLoaded += OnSceneLoaded;

        
    }

    private void OnDisable()
    {
        // Desuscribirse para evitar errores al destruir el objeto
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.ToLower() == "mainmenu")
        {
            Destroy(gameObject);
        }
    }

#endregion

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

    public void OnSpawnNewPlayer(NetworkPlayerInfo player)
    {
        Debug.Log("[Debug All] Player " + player.gameObject.name + " Connected!");
        player.SetPlayerInstanceID(players.Count);
        players.Add(player);
        if (player.IsOwner)
        {
            clientPlayerID = player.GetPlayerInstanceID();
        }

        if (IsServer)
        {
            remainingPlayerIDs.Add(player.GetPlayerInstanceID());
        }
    }    

    [ServerRpc]
    public void SetLooserServerRpc(int looserID)
    {
        if (remainingPlayerIDs.Contains(looserID))
        {
            Debug.Log("[DebugServer] Player " + players[looserID].name + " lost the game!");
            remainingPlayerIDs.Remove(looserID);
            if (remainingPlayerIDs.Count == 1)
            {
                OnEndMatch();
            }
        }
    }


    public override void OnDestroy()
    {
        remainingPlayerIDs.Dispose(); // Limpia correctamente si ya no se usa.
    }

    private void OnEndMatch()
    {
        matchesPlayed.Value+=1;
        OnEndMatchClientRpc(remainingPlayerIDs[0]);
    }

    [ClientRpc]
    private void OnEndMatchClientRpc(int winner)
    {
        if (winner == clientPlayerID)
        {
            UIManager.instance.SetWin(true);
        }else{
            UIManager.instance.SetLoose(true);
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
}
