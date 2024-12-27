using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;



public class PlayerConnection : NetworkBehaviour
{
    public static PlayerConnection instance;

    private PlayerConnectionUI connectedPlayerUI;
    public NetworkList<ConnectionData> clientInfo;
    private Dictionary<ulong, ConnectionData> pendingInfo = new Dictionary<ulong, ConnectionData>();
    
    private void Awake()
    {
        clientInfo = new NetworkList<ConnectionData>();
        if (instance != null)
        {
            instance.connectedPlayerUI = FindAnyObjectByType<PlayerConnectionUI>();
            Destroy(gameObject);
        }
        
        

        Debug.Log("AWAKE ENDED");
    }

    
    private void Start()
    {
        Debug.Log("[IMPORTANT LOG -------- ]Network spawn");
        NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApproval;
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisonnect;
        clientInfo.OnListChanged += UpdateList;
    }

    public override void OnNetworkSpawn()
    {
        if (instance == null)
        {
            Debug.Log("NETWORK SPAWNED!!");
            instance = this;
            instance.connectedPlayerUI = FindAnyObjectByType<PlayerConnectionUI>();
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;        
    }

    
    private void OnDisable()
    {   
        SceneManager.sceneLoaded -= OnSceneLoaded;    

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= OnConnectionApproval;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisonnect;
        }
        clientInfo.OnListChanged -= UpdateList;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == Utilities.SceneNames.MainMenu.ToString() || scene.name == Utilities.SceneNames.JoinMatch.ToString())
        {
            Destroy(gameObject);
        }
    }

    private void OnConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("On connection aproval!!");
        
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
        Debug.Log("Client connected!!");
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

    private void OnClientDisonnect(ulong clientID)
    {
        Debug.Log("Client disconnected!!");
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("[Server] On client disconnect call");
            RemoveClientInfo(clientID);
        }
    }

    private void AddClientInfo(ConnectionData info)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Add client info!!");
            clientInfo.Add(info);
        }
    }

    private void RemoveClientInfo(ulong clientID)
    {
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
    public void PrintInfo()
    {
        Debug.Log("List updated");
        foreach (var val in clientInfo)
        {
            Debug.Log("CLIENT: "+val.clientID+" username: "+val.username+" skin id: "+val.skinId);
        }
    }

    private void UpdateList(NetworkListEvent<ConnectionData> changeEvent)
    {
        PrintInfo();
        if (connectedPlayerUI != null)
        {
            connectedPlayerUI.OnListUpdate();
        }
    }

}
