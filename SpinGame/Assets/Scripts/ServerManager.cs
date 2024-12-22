using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using System.Net;


public class ServerManager : MonoBehaviour
{

#if UNITY_SERVER
    // Start is called before the first frame update
    void Start()
    {

        if (SceneManager.GetActiveScene().name == Utilities.SceneNames.MainMenu.ToString())
        {
            SceneManager.LoadScene(Utilities.SceneNames.MatchMaking.ToString());
        }
        else if (SceneManager.GetActiveScene().name == Utilities.SceneNames.MatchMaking.ToString())
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.OnServerStarted += OnServerStarted;
                string ip = GetLocalIPAddress();
                if (ip == "localhost")
                {
                    Debug.Log("[SERVER WARNING] RUNNING ON LOCALHOST");
                }
                ConfigureTransport(ip, 7777);
                NetworkManager.Singleton.StartServer();
            }
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

    private void OnDisable()
    {
        if (NetworkManager.Singleton)
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
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
        }
    }


#endif
}
