using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button [] buttons;

    [SerializeField] private UIPlayerConnection playerConnectionUI;


    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;
    

    

    private void Awake()
    {
        buttons[0].onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer()       
            ;});
        buttons[1].onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            });
        buttons[2].onClick.AddListener(() => {
            string ip = ipInput.text;
            ushort port = ushort.Parse(portInput.text);
            ConfigureTransport(ip, port);
            NetworkManager.Singleton.StartClient();
            });
    }

    void ConfigureTransport(string ip, ushort port)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ip; // Escuchar en todas las interfaces
        transport.ConnectionData.Port = port;        // Puerto personalizado
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback+= AddPlayerToList;
            NetworkManager.Singleton.OnClientDisconnectCallback+= RemovePlayerFromList;
        }
            
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback-= AddPlayerToList;
            NetworkManager.Singleton.OnClientDisconnectCallback-= RemovePlayerFromList;
        }
        
    }

    private void AddPlayerToList(ulong clientID)
    {
        Debug.Log("UI CONNECTING PLAYER STEP 1");
        playerConnectionUI.ConnectPlayer("Player "+NetworkManager.Singleton.ConnectedClientsIds.Count.ToString(), clientID);
    }

    private void RemovePlayerFromList(ulong clientID)
    {
        Debug.Log("UI REMOVING PLAYER STEP 1");
        playerConnectionUI.DisconnectPlayer(clientID);
    }

}
