using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Linq;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
public class SessionManager : NetworkBehaviour
{
    public event Action<string> StateMessageEvents;
    public event Action onClientStartConnection;
    public event Action onClientStopConnection;

    struct CreateMatchData{
        public string ip;
        public int port;
    };

    public float connectionTimeout = 5f; 
    private bool isTryingToConnect = false;

    Coroutine connectingCoroutine;

    public void StartClientConnection()
    {
        if (isTryingToConnect) { return; }

        isTryingToConnect = true;
        connectingCoroutine = StartCoroutine(TryConnect());
    }

    private IEnumerator TryConnect()
    {
        try{

            NetworkManager.Singleton.StartClient();
        }catch(Exception error)
        {
            Debug.Log("ERROR!!!"+error.Message);
        }
        onClientStartConnection?.Invoke();

        float startTime = Time.time;

        Debug.Log("Try connect 1");
        while (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.Log("Try connect 2");
            if (Time.time - startTime >= connectionTimeout)
            {
                
                StopConnectionAttempt();
                yield break;
            }
            yield return null;
        }
        onClientStopConnection?.Invoke();
        isTryingToConnect = false;
    }

    private void StopConnectionAttempt()
    {
        Debug.Log("Try connect 3");
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.Shutdown();
        }
        isTryingToConnect = false;
        onClientStopConnection?.Invoke();
        StateMessageEvents?.Invoke("Could not connect to the game");
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientStarted-=OnClientStarted;
            NetworkManager.Singleton.OnServerStarted-=OnServerStarted;
            NetworkManager.Singleton.OnClientDisconnectCallback-= OnClientDisonnect;
        }
    }

    public void ConnectServer()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientStarted+=OnClientStarted;
            NetworkManager.Singleton.OnClientDisconnectCallback+= OnClientDisonnect;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;

            string ip = GetLocalIPAddress();
            var args = System.Environment.GetCommandLineArgs();

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
            NetworkManager.Singleton.SceneManager.LoadScene(Utilities.SceneNames.MatchMaking.ToString(), LoadSceneMode.Single);
        }
    }


    void ConfigureTransport(string ip, ushort port)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ip; 
        try{
            transport.ConnectionData.Port = port;

        }catch{
            Debug.Log("HERE");
        }
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

    
    public void ConnectClientToMatch(string ip, string portString, string _username)
    {
        var connectionData = new {
            username = _username,
            skinid = 1
            };
        
        string jsonData = JsonConvert.SerializeObject(connectionData);

        Debug.Log("DATA IN JSON: "+jsonData);
        byte[] payload = Encoding.UTF8.GetBytes(jsonData);

        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectTimeoutMS = 1000;

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payload;
        NetworkManager.Singleton.OnClientDisconnectCallback+= OnClientDisonnect;

        ushort port = 0;
        try{
            port = ushort.Parse(portString);
            
        }catch{
            StateMessageEvents?.Invoke("The code is not valid!");
        }
        if (port > 0)
        {
            ConfigureTransport(ip, port);
            StartClientConnection();
        }
    }



    public void CreateMatch(string ip, string username)
    {
        StartCoroutine(CreateMatchCoroutine(ip, username));
    }

    IEnumerator CreateMatchCoroutine(string ip, string username)
    {
        string url = "http://"+ip+":5100/MatchMaking/CreateMatch";
        //string url = "http://"+"192.168.0.104"+":5100/MatchMaking/CreateMatch";
        Debug.Log(url);

        
        onClientStartConnection?.Invoke();
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                //Debug.LogError($"Error: {webRequest.error}");
                StateMessageEvents?.Invoke("Servers are currently unavailable");
                onClientStopConnection?.Invoke();
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log($"Respuesta JSON: {jsonResponse}");
                CreateMatchData data = JsonUtility.FromJson<CreateMatchData>(jsonResponse);
                ConnectClientToMatch(ip, data.port.ToString(), username);
            }
            
        }
    }


    private void OnClientDisonnect(ulong clientID)
    {
        Debug.Log("On client disconnect, id: "+clientID);
        if (!NetworkManager.Singleton.IsServer && NetworkManager.Singleton.DisconnectReason != string.Empty)
        {
            Debug.Log("Player "+clientID+ " disconnected Reason: "+NetworkManager.Singleton.DisconnectReason);
            StateMessageEvents?.Invoke(NetworkManager.Singleton.DisconnectReason);
            if (connectingCoroutine != null)
            {
                StopCoroutine(connectingCoroutine);
            }
        }
    }


    private void OnClientStarted()
    {
        Debug.Log("Client started");
    }


    



}
