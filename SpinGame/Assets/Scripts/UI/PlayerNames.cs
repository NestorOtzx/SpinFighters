using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNames : MonoBehaviour
{
    [SerializeField] private GameObject playerNamePref;

    Dictionary<ulong, PlayerNameUI> names = new Dictionary<ulong, PlayerNameUI>();
    void Start()
    {
        GameManager.instance.OnPlayerLost += OnPlayerLost;
        StartCoroutine(LateStart());
    }

    private void OnDisable()
    {
        GameManager.instance.OnPlayerLost -= OnPlayerLost;
    }

    private IEnumerator LateStart()
    {
        
        yield return new WaitForSeconds(0.1f);
        

        if (GameManager.instance.isSinglePlayer)
        {
            foreach(var info in PlayerConnection.instance.clientInfoSingle )
            {   
                Debug.Log("Setting name "+info.username);
                GameObject obj = Instantiate(playerNamePref, transform); 
                PlayerNameUI playerNameUI = obj.GetComponent<PlayerNameUI>();
                playerNameUI.SetTarget(GameManager.instance.players[info.clientID].transform);
                playerNameUI.SetName(info.username.ToString());
                if (info.clientID == GameManager.instance.clientPlayerID)
                {   
                    playerNameUI.SetPlayerIndicator(true);
                }else{
                    playerNameUI.SetPlayerIndicator(false);
                }


                names.Add(info.clientID, playerNameUI);
            }
        }else {
            foreach(var info in PlayerConnection.instance.clientInfo)
            {        
                GameObject obj = Instantiate(playerNamePref, transform); 
                PlayerNameUI playerNameUI = obj.GetComponent<PlayerNameUI>();
                playerNameUI.SetTarget(GameManager.instance.players[info.clientID].transform);
                playerNameUI.SetName(info.username.ToString());
                if (info.clientID == GameManager.instance.clientPlayerID)
                {   
                    playerNameUI.SetPlayerIndicator(true);
                }else{
                    playerNameUI.SetPlayerIndicator(false);
                }
                names.Add(info.clientID, playerNameUI);
            }
        }

        yield return new WaitForSeconds(1.5f);
        ShowNames(false);
    }

    private void Update()
    {
        if (names.Count>0)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ShowNames(true);
            }
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                ShowNames(false);
            }
        }
    }

    private void ShowNames(bool t)
    {
        foreach(var n in names.Values)
        {
            n.Show(t);
        }
    }

    public void OnPlayerLost(ulong playerID)
    {
        if (names.ContainsKey(playerID) && names[playerID] != null)
        {
            Destroy(names[playerID].gameObject);
        }
    }
}
