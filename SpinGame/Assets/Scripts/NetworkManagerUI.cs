using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button [] buttons;

    [SerializeField] private UIPlayerConnection playerConnectionUI;

    

    private void Awake()
    {
        buttons[0].onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer()       
            ;});
        buttons[1].onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            });
        buttons[2].onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            });
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
        Debug.Log("Connect player 0");
        playerConnectionUI.ConnectPlayer("Player "+NetworkManager.Singleton.ConnectedClientsIds.Count.ToString(), clientID);
    }

    private void RemovePlayerFromList(ulong clientID)
    {
        Debug.Log("Disconnect player 0");
        playerConnectionUI.DisconnectPlayer(clientID);
    }

}
