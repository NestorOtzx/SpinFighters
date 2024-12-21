using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using Unity.VisualScripting;

public class UIPlayerConnection : NetworkBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform prefabsContainer;


    public NetworkList<ulong> clientIDs;
    public NetworkList<FixedString32Bytes> clientNames;

    private Dictionary<ulong, GameObject> uiPlayers = new Dictionary<ulong, GameObject>();
    private Dictionary<ulong, TextMeshProUGUI> uiPlayerTexts = new Dictionary<ulong, TextMeshProUGUI>();

    private void Awake()
    {
        clientIDs = new NetworkList<ulong>();
        clientNames = new NetworkList<FixedString32Bytes>();

        if ( NetworkManager.Singleton)
        {
            int count = 1;
            foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                ConnectPlayer("Player "+count.ToString(), id);
                count++;
            }
        }
    }

    private void OnEnable()
    {
        clientNames.OnListChanged += UpdateName;
    }

    private void OnDisable()
    {
        clientNames.OnListChanged -= UpdateName;
    }
    public void ConnectPlayer(string playerName, ulong id)
    {
        Debug.Log("Connect player 2");
        if (IsServer)   
            AddClientsServerRpc(id, playerName);   
    }

    public void DisconnectPlayer(ulong id)
    {
        Debug.Log("Connect player 2");
        if (IsServer)   
            RemoveClientsServerRpc(id);
    }

    [ServerRpc]
    private void AddClientsServerRpc(ulong id, string playername)
    {
        clientIDs.Add(id);
        clientNames.Add(playername);
    }

    [ServerRpc]
    private void RemoveClientsServerRpc(ulong id)
    {
        int index = clientIDs.IndexOf(id);
        clientIDs.RemoveAt(index);
        clientNames.RemoveAt(index);
    }

    private void UpdateName(NetworkListEvent<FixedString32Bytes> changeEvent)
    {
        
        foreach (ulong key in uiPlayers.Keys)
        {
            Destroy(uiPlayers[key]);
        }
        uiPlayers.Clear();
        uiPlayerTexts.Clear();
        Debug.Log("Los nombres cambiaron!!");
        for (int i = 0; i< clientNames.Count; i++)
        {
            GameObject obj = Instantiate(prefab, prefabsContainer);
            uiPlayers.Add(clientIDs[i], obj);
            TextMeshProUGUI tm = obj.GetComponentInChildren<TextMeshProUGUI>();
            uiPlayerTexts.Add(clientIDs[i], tm);
            tm.text = clientNames[i].ToString();
            Debug.Log("Jugador: "+clientNames[i] + " client id: "+ clientIDs[i]);
        }
    }
}
