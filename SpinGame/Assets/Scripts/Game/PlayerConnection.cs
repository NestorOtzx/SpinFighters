using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;



public class PlayerConnection : NetworkBehaviour
{
    public static PlayerConnection instance;
    public NetworkList<ConnectionData> clientInfo;
    private Dictionary<ulong, ConnectionData> pendingInfo = new Dictionary<ulong, ConnectionData>();

    public List<ConnectionData> clientInfoSingle;
    public event Action OnHostChanged;
    public event Action OnClientsUpdate;

    private void Awake()
    {
        Debug.Log("[PlayerConnection] Awake");
        clientInfo = new NetworkList<ConnectionData>();
        if (instance != null)
        {
            foreach (var v in instance.clientInfo)
            {
                ConnectionData data = v;
                data.score = 0;
                pendingInfo.Add(v.clientID, data);
            }            
            Destroy(instance.gameObject);
            instance = null;
        }
        if (SceneManager.GetActiveScene().name == Utilities.SceneNames.SinglePlayer.ToString())
        {
            if (instance == null)
            {
                Debug.Log("[PlayerConnection] Scene loaded");
                instance = this;
        
                foreach (var k in pendingInfo.Keys)
                {
                    AddClientInfo(pendingInfo[k]);
                }
                pendingInfo.Clear();

                DontDestroyOnLoad(gameObject);
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("[PlayerConnection] Network Spawn");
        if (instance == null)
        {
            Debug.Log("[PlayerConnection] Network Spawn - Get connected player UI");
            instance = this;
    
            foreach (var k in pendingInfo.Keys)
            {
                AddClientInfo(pendingInfo[k]);
            }
            pendingInfo.Clear();

            DontDestroyOnLoad(gameObject);
        }
    }
    
    private void Start()
    {
        Debug.Log("[PlayerConnection] Start");
        if (NetworkManager.Singleton)
        {

        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApproval;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisonnect;
        clientInfo.OnListChanged += UpdateClientList;
        }
    }

    

    private void OnEnable()
    {
        Debug.Log("[PlayerConnection] On Enable");
        SceneManager.sceneLoaded += OnSceneLoaded;        
    }

    private void SetHost()
    {
        Debug.Log("Seto host 1 ");
        if (GameManager.instance.isSinglePlayer)
        {
            foreach (var v in clientInfoSingle)
            {
                if (v.isHost)
                {
                    OnHostChanged?.Invoke();
                    return; //host exists!
                }
            }
        }else{
             foreach (var v in clientInfo)
            {
                if (v.isHost)
                {
                    OnHostChanged?.Invoke();
                    return; //host exists!
                }
            }
        }

        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Seto host 2 ");
            if (clientInfo.Count > 0)
            {
                Debug.Log("Seto host 3 ");
                int index = UnityEngine.Random.Range(0, clientInfo.Count);
                var val = clientInfo[index];
                Debug.Log("Seto host 4 "+val.username);
                val.isHost = true;
                clientInfo[index] = val; 
            }
        }
        OnHostChanged?.Invoke();
    }

    
    private void OnDisable()
    {   
        Debug.Log("[PlayerConnection] On Disable");
        SceneManager.sceneLoaded -= OnSceneLoaded;    

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= OnConnectionApproval;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisonnect;
            clientInfo.OnListChanged -= UpdateClientList;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[PlayerConnection] OnSceneLoaded");
        if (scene.name == Utilities.SceneNames.MainMenu.ToString() || scene.name == Utilities.SceneNames.JoinMatch.ToString())
        {
            Destroy(gameObject);
        }
    }

    private void OnConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("[PlayerConnection] On Connection approval");
        
        string jsonData = Encoding.UTF8.GetString(request.Payload);

        var connectionData = JsonConvert.DeserializeObject<ConnectionData>(jsonData);

        Debug.Log($"Client {request.ClientNetworkId} data ( username:  {connectionData.username}, skin: {connectionData.skinId}");

        // Aprobar la conexión
        response.Approved = true;
        response.CreatePlayerObject = false;

        // Guardar los datos
        if (response.Approved && NetworkManager.Singleton.IsServer)
        {   
            PrintInfo();
            ConnectionData data = new ConnectionData(request.ClientNetworkId, connectionData.username, connectionData.skinId);
            pendingInfo.Add(request.ClientNetworkId, data);
        }
    }

    private void OnClientConnect(ulong clientID)
    {
        Debug.Log("[PlayerConnection] On Client Connect");
        if (NetworkManager.Singleton.IsServer)
        {
            if (pendingInfo.ContainsKey(clientID))
            {
                Debug.Log("[Server] On client connect call");
                AddClientInfo(pendingInfo[clientID]);
                pendingInfo.Remove(clientID);

                
            }
        }
    }

    public void ConnectSinglePlayer(string name, int skinID, bool isHost)
    {
        Debug.Log("Connect player, player count: "+(ulong)clientInfoSingle.Count);
        
        ConnectionData data = new ConnectionData((ulong)clientInfoSingle.Count, name, skinID);
        data.isHost = isHost;
        AddClientInfo(data);
    }

    private void OnClientDisonnect(ulong clientID)
    {
        Debug.Log("[PlayerConnection] On Client Disconnect");
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("[Server] On client disconnect call");
            RemoveClientInfo(clientID);
        }
    }

    public void DisconnectSinglePlayer(ulong botID)
    {
        RemoveClientInfo(botID);
    }

    private void AddClientInfo(ConnectionData info)
    {
        if (!NetworkManager.Singleton) //is single player
        {
            clientInfoSingle.Add(info);
            OnClientsUpdate?.Invoke();
        }
        else if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Add client info!!");
            clientInfo.Add(info);
        } 
    }

    private void RemoveClientInfo(ulong clientID)
    {
        if (GameManager.instance.isSinglePlayer)
        {
            int index = -1;
            for (int i = 0; i<clientInfoSingle.Count ;i++)
            {
                if (clientInfoSingle[i].clientID == clientID)
                {
                    index = i;
                    break;
                }
            }
            if (index > -1)
            {
                Debug.Log("Remove client!!");
                clientInfoSingle.RemoveAt(index);
            }
            OnClientsUpdate?.Invoke();
        }else{

            int index = -1;
            for (int i = 0; i<clientInfo.Count ;i++)
            {
                if (clientInfo[i].clientID == clientID)
                {
                    index = i;
                    break;
                }
            }
            if (index > -1)
            {
                Debug.Log("Remove client!!");
                clientInfo.RemoveAt(index);
            }
        }
    }
    public void PrintInfo()
    {
        Debug.Log("List updated");
        foreach (var val in clientInfo)
        {
            Debug.Log("CLIENT: "+val.clientID+" username: "+val.username+" skin id: "+val.skinId);
        }
    }

    private void UpdateClientList(NetworkListEvent<ConnectionData> changeEvent)
    {
        PrintInfo();
        Debug.Log("Clients number: "+clientInfo.Count+ "event type: "+changeEvent.Type.ToString());
        SetHost();

        OnClientsUpdate?.Invoke();
    }

    public void AddScore(ulong clientID)
    {
        Debug.Log("Add score is single player"+clientID);
        if (GameManager.instance.isSinglePlayer)
        {
            int clientIndex = 0;
            for (int i = 0; i<clientInfoSingle.Count; i++ )
            {
                if (clientInfoSingle[i].clientID == clientID)
                {
                    clientIndex = i;
                    break;
                }
            }

            var prevClient = clientInfoSingle[clientIndex];
            prevClient.score+=1;
            clientInfoSingle[clientIndex] = prevClient;
            OnClientsUpdate?.Invoke();
        }
        else if (NetworkManager.Singleton.IsServer)
        {
            int clientIndex = 0;
            for (int i = 0; i<clientInfo.Count; i++ )
            {
                if (clientInfo[i].clientID == clientID)
                {
                    clientIndex = i;
                    break;
                }
            }

            var prevClient = clientInfo[clientIndex];
            prevClient.score+=1;
            clientInfo[clientIndex] = prevClient;
        }
    }

    public int GetMaxScore()
    {
        int maxScore = -1;
        if (GameManager.instance.isSinglePlayer)
        {
            foreach (var v in clientInfoSingle)
            {
                if (v.score > maxScore)
                {
                    maxScore = v.score;
                }
            }
            
        }else{
            foreach (var v in clientInfo)
            {
                if (v.score > maxScore)
                {
                    maxScore = v.score;
                }
            }
        }
        return maxScore;
    }

    public ConnectionData GetWinner()
    {
        ConnectionData client;
        if (GameManager.instance.isSinglePlayer)
        {
            client = clientInfoSingle[0];
            int maxScore = GetMaxScore();

            for (int i = 0; i<clientInfoSingle.Count; i++ )
            {
                if (clientInfoSingle[i].score == maxScore)
                {
                    client = clientInfoSingle[i];
                }
            }
        }else{

            client = clientInfo[0];
            int maxScore = GetMaxScore();

            for (int i = 0; i<clientInfo.Count; i++ )
            {
                if (clientInfo[i].score == maxScore)
                {
                    client = clientInfo[i];
                }
            }
        }
        return client;
    }

    public bool CheckDraw()
    {

        int maxScore = GetMaxScore();
        int winnersCount = 0;

        if (GameManager.instance.isSinglePlayer)
        {
            for (int i = 0; i<clientInfoSingle.Count; i++ )
            {
                if (clientInfoSingle[i].score == maxScore)
                {
                    winnersCount++;
                }
            }
        }else{

            for (int i = 0; i<clientInfo.Count; i++ )
            {
                if (clientInfo[i].score == maxScore)
                {
                    winnersCount++;
                }
            }
        }
        return winnersCount > 1;
    }
}
