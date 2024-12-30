using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HostActiveObjects : MonoBehaviour
{
    public GameObject [] hostVisibleObjects;
    public GameObject [] clientVisibleObjects;


    void Start()
    {
        PlayerConnection.instance.OnHostChanged += RefreshObjects;
        Debug.Log("Host start");
        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
        
        RefreshObjects();

    }

    private int GetCurrentHost(){
        if (GameManager.instance.isSinglePlayer)
        {
            foreach (var client in PlayerConnection.instance.clientInfoSingle)
            {
                if (client.isHost)
                {
                    return (int)client.clientID;
                }
            }
            
        }else{
            foreach (var client in PlayerConnection.instance.clientInfo)
            {
                if (client.isHost)
                {
                    return (int)client.clientID;
                }
            }
        }
        return -1;
    }

    void OnDisable()
    {
        if (PlayerConnection.instance)
            PlayerConnection.instance.OnHostChanged -= RefreshObjects;
    }

    private void RefreshObjects()
    {
        int currentHost = GetCurrentHost();
        if (currentHost > -1)
        {
            Debug.Log("REFRESH HOST");

            bool isHost = (GameManager.instance.isSinglePlayer && currentHost == 0) || (NetworkManager.Singleton.LocalClientId == (ulong)currentHost);

            foreach (GameObject obj in hostVisibleObjects)
            {
                obj.SetActive(isHost);
            }
            foreach (GameObject obj in clientVisibleObjects)
            {
                obj.SetActive(!isHost);
            }
        }
    }
}
