using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPlayerConnection : NetworkBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform prefabsContainer;

    private NetworkList<ulong> clientIDs;
    private NetworkList<FixedString128Bytes> clientNames;

    private bool idsUpdated = true, namesUpdated = true; 


    private Dictionary<ulong, GameObject> prefInstances = new Dictionary<ulong, GameObject>();

     private void Awake()
    {
        // Inicializar la lista en el constructor, pero no modificarla aún.
        Debug.Log("[IMPORTANT LOG -------- ]Call start");
        clientIDs = new NetworkList<ulong>();
        clientNames = new NetworkList<FixedString128Bytes>();
    }



    private void Start()
    {
        Debug.Log("[IMPORTANT LOG -------- ]Network spawn");
        clientIDs.OnListChanged+=UpdateId;
        clientNames.OnListChanged += UpdateName;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisonnect;
        if (prefInstances.Count != clientIDs.Count)
        {
            idsUpdated = true;
            namesUpdated = true;
            UpdateList();
        }
    }

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            foreach(ulong clientid in NetworkManager.Singleton.ConnectedClientsIds){
                if (!clientIDs.Contains(clientid))
                {
                    AddPlayerToList("Player"+clientid.ToString(), clientid);
                }
            }
        }
    }
    private void OnDisable()
    {   
        clientIDs.OnListChanged-=UpdateId;
        clientNames.OnListChanged -= UpdateName;
       
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisonnect;
        }
    }

    private void OnDestroy()
    {
        clientIDs.Dispose();
        clientNames.Dispose();
    }

    private void OnClientConnect(ulong clientID)
    {

        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("[Server] On client connect call");
            AddPlayerToList("Player "+clientID.ToString(), clientID);
        }
        
    }

    private void OnClientDisonnect(ulong clientID)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("[Server] On client disconnect call");
            RemovePlayerFromList(clientID);
        }
    }


    private void UpdateId(NetworkListEvent<ulong> changeEvent)
    {
        idsUpdated = true;
        UpdateList();
    }
    private void UpdateName(NetworkListEvent<FixedString128Bytes> changeEvent){
        namesUpdated = true;
        UpdateList();
    }
    private void UpdateList()
    {
        if (idsUpdated && namesUpdated)
        {
            foreach(var key in prefInstances.Keys)
            {
                Destroy(prefInstances[key]);
            }
            prefInstances.Clear();
            Debug.Log("Update All players");
            for(int i = 0; i<clientNames.Count; i++)
            {
                GameObject obj = Instantiate(prefab, prefabsContainer);
                TextMeshProUGUI tm = obj.GetComponentInChildren<TextMeshProUGUI>();
                tm.text = clientNames[i].ToString();
                prefInstances.Add(clientIDs[i], obj);
                Debug.Log("client: "+ clientNames[i]);
            }
        }
    }


    private void AddPlayerToList(string playerName, ulong id)
    {
        clientIDs.Add(id); 
        clientNames.Add(playerName);//Allways change last
        idsUpdated = false;
        namesUpdated = false;
    }

    private void RemovePlayerFromList(ulong id)
    {
        int index  = clientIDs.IndexOf(id);
        clientIDs.RemoveAt(index); 
        clientNames.RemoveAt(index);//Allways change last
        idsUpdated = false;
        namesUpdated = false;
    }



}
