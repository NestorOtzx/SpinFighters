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
        Debug.Log("[PlayerConnection] Awake");
        clientInfo = new NetworkList<ConnectionData>();
        if (instance != null)
        {
            foreach (var v in instance.clientInfo)
            {
                pendingInfo.Add(v.clientID, v);
            }            
            Destroy(instance.gameObject);
            instance = null;
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

            instance.connectedPlayerUI = FindAnyObjectByType<PlayerConnectionUI>();
            DontDestroyOnLoad(gameObject);
        }
    }
    
    private void Start()
    {
        Debug.Log("[PlayerConnection] Start");
        NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApproval;
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisonnect;
        clientInfo.OnListChanged += UpdateList;
    }

    

    private void OnEnable()
    {
        Debug.Log("[PlayerConnection] On Enable");
        SceneManager.sceneLoaded += OnSceneLoaded;        
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
        }
        clientInfo.OnListChanged -= UpdateList;
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

    private void OnClientDisonnect(ulong clientID)
    {
        Debug.Log("[PlayerConnection] On Client Disconnect");
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
