using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Linq;
using System;
using UnityEngine.Networking;
public class SessionManager : NetworkBehaviour
{

    public static SessionManager instance;


    struct CreateMatchData{
        public string ip;
        public int port;
    };

    private void Awake()
    {

        // Verificar si ya existe otro NetworkManager activo
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else if (SceneManager.GetActiveScene().name == Utilities.SceneNames.MainMenu.ToString()) //en el main menu no habrá sesiones
        {
            Destroy(gameObject);
        }
        else{
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton)
        {
            
        }   
    }

    private void OnDisable()
    {

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientStarted-=OnClientStarted;
            NetworkManager.Singleton.OnServerStarted-=OnServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback-= OnClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback-= OnClientDisonnect;
        }
    }

    public void ConnectServer()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientStarted+=OnClientStarted;
            NetworkManager.Singleton.OnClientConnectedCallback+= OnClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback+= OnClientDisonnect;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            string ip = GetLocalIPAddress();
            var args = System.Environment.GetCommandLineArgs();
            // Configurar puerto basado en el argumento
            var portArg = args.FirstOrDefault(arg => arg.StartsWith("-port"));
            ushort port = 7777;
            if (portArg != null)
            {
                port = ushort.Parse(portArg.Split(' ')[1]);
            }

            ConfigureTransport(ip, port);
            NetworkManager.Singleton.StartServer();
        }
    }

    private void OnServerStarted()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("El servidor ha sido inicializado!");
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            ushort port = transport.ConnectionData.Port;
            string ipAddress = transport.ConnectionData.Address;

            Debug.Log($"El servidor está corriendo en la IP: {ipAddress}, Puerto: {port}");
            //SceneManager.LoadScene(Utilities.SceneNames.MatchMaking.ToString());
        }
    }


    void ConfigureTransport(string ip, ushort port)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ip; // Escuchar en todas las interfaces
        transport.ConnectionData.Port = port;        // Puerto personalizado
    }

    string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "localhost";
    }

    
    public void ConnectClientToMatch(string ip, ushort port)
    {
        ConfigureTransport(ip, port);
        NetworkManager.Singleton.StartClient();
    }

    public void CreateMatch(string ip)
    {
        StartCoroutine(CreateMatchCoroutine(ip));
    }

    IEnumerator CreateMatchCoroutine(string ip)
    {
        string url = "http://"+ip+":5100/MatchMaking/CreateMatch";
        Debug.Log(url);

        // Crear la solicitud
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Enviar la solicitud y esperar la respuesta
            yield return webRequest.SendWebRequest();

            // Manejo de errores
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log($"Respuesta JSON: {jsonResponse}");
                CreateMatchData data = JsonUtility.FromJson<CreateMatchData>(jsonResponse);
                ConfigureTransport(ip, (ushort)data.port);
                NetworkManager.Singleton.StartClient();
            }
        }
    }



    private void OnClientConnect(ulong clientID)
    {
        Debug.Log("Player "+clientID+ " connected");
        //currentPort.text = "Port: "+transport.ConnectionData.Port;
        //playerConnectionUI.ConnectPlayer("Player "+NetworkManager.Singleton.ConnectedClientsIds.Count.ToString(), clientID);
    }

    [ServerRpc(RequireOwnership =false)]
    public void ClientConnectedServerRpc(ulong clientID)
    {

    }

    [ClientRpc]
    public void ClientConnectedClientRpc(ulong clientID)
    {
        

    }

    private void OnClientDisonnect(ulong clientID)
    {
        Debug.Log("Player "+clientID+ " disconnected");
        //playerConnectionUI.DisconnectPlayer(clientID);
    }


    private void OnClientStarted()
    {
        Debug.Log("Client started");
    }


    



}