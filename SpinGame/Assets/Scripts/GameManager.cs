using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public Dictionary<ulong, PlayerInfo> players = new Dictionary<ulong, PlayerInfo>();
    public NetworkList<ulong> remainingPlayerIDs;

    public List<ulong> remainingPlayersSingle;
    public int numberOfRounds = 3;
    private NetworkVariable<int> roundsPlayed = new NetworkVariable<int>(0);
    private int roundsPlayedSingle;

    public ulong clientPlayerID = 666666;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject botPrefab;

    private Transform [] playerSpawns;

    public bool isSinglePlayer;

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

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    public void SetSpawns(Transform [] spawns)
    {
        playerSpawns = spawns;
    }


    //Each client should call this function so their respective players get spawned.
    public void SpawnPlayerSrv(ulong clientId, int spawn_id)
    {
        //Instantiate player
        GameObject playerInstance = Instantiate(playerPrefab, playerSpawns[spawn_id].position, Quaternion.identity);
        //Spawn on network
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    public void SpawnSinglePlayer()
    {
        GameObject playerInstance = Instantiate(playerPrefab, playerSpawns[0].position, Quaternion.identity);
        playerInstance.GetComponent<PlayerInfo>().SetId(0);
        
        //Instantiate bots
        for(int i=1; i<Mathf.Min(PlayerConnection.instance.clientInfoSingle.Count, playerSpawns.Length); i++)
        {
            GameObject obj = Instantiate(botPrefab, playerSpawns[i].position, Quaternion.identity);
            obj.GetComponent<PlayerInfo>().SetId(PlayerConnection.instance.clientInfoSingle[i].clientID);
        }
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
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == Utilities.SceneNames.MainMenu.ToString() || scene.name == Utilities.SceneNames.JoinMatch.ToString())
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
        if (scene.name == Utilities.SceneNames.SinglePlayer.ToString())
        {
            Debug.Log("[GameManager] Scene loaded");
            isSinglePlayer = true;
            PlayerConnection.instance.ConnectSinglePlayer("Player", 0, true);
            
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }   
    

    public void OnSpawnNewPlayer(PlayerInfo player)
    {
        Debug.Log("Spawn player, is owner: "+player.IsOwner+", playerid:"+player.playerID);
        players.Add(player.playerID, player);
        if (isSinglePlayer && player.gameObject.CompareTag("Player")) 
        {
            clientPlayerID = player.playerID;
        }
        else if (player.IsOwner)
        {
            clientPlayerID = player.playerID;
        }

        if (isSinglePlayer)
        {
            remainingPlayersSingle.Add(player.playerID);
        }
        else if (NetworkManager.Singleton.IsServer)
        {
            remainingPlayerIDs.Add(player.playerID);
        }
    }    

    public void SetLooserSrv(ulong looserID)
    {
        Debug.Log("Set looser "+looserID);
        if (isSinglePlayer)
        {
            foreach (var el in remainingPlayersSingle)
            {
                Debug.Log("remaining player: "+el);
            }
            if (remainingPlayersSingle.Contains(looserID))
            {
                Debug.Log("[DebugServer] Player " + players[looserID].name + " lost the game!");
                remainingPlayersSingle.Remove(looserID);
                if (remainingPlayersSingle.Count == 1)
                {
                    OnEndRound();
                }
            }
        }else{
            foreach (var el in remainingPlayerIDs)
            {
                Debug.Log("remaining player: "+el);
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
    }


    public override void OnDestroy()
    {
        remainingPlayerIDs.Dispose(); // Limpia correctamente si ya no se usa.
    }

    private void OnEndRound()
    {   
        List<PlayerInfo> playerCopy;
        if (isSinglePlayer)
        {
            roundsPlayedSingle += 1;
            playerCopy = new List<PlayerInfo>(players.Values);
            PlayerConnection.instance.AddScore(remainingPlayersSingle[0]); 
            ShowGameWinner(remainingPlayersSingle[0]);
            remainingPlayersSingle.Clear();
        }else{
            roundsPlayed.Value+=1;
            playerCopy = new List<PlayerInfo>(players.Values);
            PlayerConnection.instance.AddScore(remainingPlayerIDs[0]); 
            OnEndRoundClientRpc(remainingPlayerIDs[0]);
            remainingPlayerIDs.Clear();
        }
        
        
        players.Clear();
        StartCoroutine(NextRound(playerCopy));
    }

    [ClientRpc]
    private void OnEndRoundClientRpc(ulong winner)
    {
        ShowGameWinner(winner);
    }

    private void ShowGameWinner(ulong winner)
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

    [ClientRpc]
    private void SetDrawUIClientRpc()
    {
        UIManager.instance.SetDraw();
    }

    IEnumerator NextRound(List<PlayerInfo> playerCopy)
    {
        if (instance)
        {
            yield return new WaitForSeconds(1);

            int rp;
            if (isSinglePlayer)
            {
                rp = roundsPlayedSingle;
            }else{
                rp = roundsPlayed.Value;
            }
            if (rp < instance.numberOfRounds && (isSinglePlayer || NetworkManager.Singleton.ConnectedClientsIds.Count > 1))
            {
                instance.LoadGameScene(SceneManager.GetActiveScene().name);
            }else{
                if (PlayerConnection.instance.CheckDraw())
                {
                    if (isSinglePlayer)
                    {
                        UIManager.instance.SetDraw();
                    }else{
                        SetDrawUIClientRpc();
                    }
                    yield return new WaitForSeconds(1);
                    instance.LoadGameScene(SceneManager.GetActiveScene().name);
                }else{
                    instance.LoadGameScene(Utilities.SceneNames.ScoresScreen.ToString());
                }
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
        if (isSinglePlayer)
        {
            SceneManager.LoadScene(name);     
        }else{
            if (NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(name, LoadSceneMode.Single);
            }
        }      
    }

    public void RestartMatch()
    {
        if (isSinglePlayer)
        {
            roundsPlayedSingle = 0;
        }
        else if (NetworkManager.Singleton.IsServer)
        {
            roundsPlayed.Value = 0;
        }
    }

    private void OnClientDisconnect(ulong clientId)
    {
        
        if (NetworkManager.Singleton.IsServer)
        {
            bool isInLevels = SceneManager.GetActiveScene().name != Utilities.SceneNames.MatchMaking.ToString() && SceneManager.GetActiveScene().name != Utilities.SceneNames.ScoresScreen.ToString();
            int clientCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            Debug.Log("Client "+clientId+" disconnected, remaining clients: "+clientCount);
            if (isInLevels && clientCount > 0)
            {
                SetLooserSrv(clientId);
            }
            if (clientCount == 0){
                Application.Quit(); //close server due to there is no players using it :)
            }
        }else if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("[Client] Disconnected from server");
            SceneManager.LoadScene(Utilities.SceneNames.JoinMatch.ToString());
        }
    }

    public void SetNumberOfRounds(int value)
    {
        if (isSinglePlayer)
        {
            instance.numberOfRounds = value;
        }else{
            SetNumberOfRoundsServerRpc(value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetNumberOfRoundsServerRpc(int value)
    {
        instance.numberOfRounds = value;
        SetNumberOfRoundsClientRpc(value);
    }

    [ClientRpc]
    private void SetNumberOfRoundsClientRpc(int value)
    {
        instance.numberOfRounds = value;
    }
}
