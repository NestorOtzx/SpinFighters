using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Linq;


public class ServerManager : MonoBehaviour
{

#if UNITY_SERVER
    // Start is called before the first frame update
    void Start()
    {

        if (SceneManager.GetActiveScene().name == Utilities.SceneNames.MainMenu.ToString())
        {
            SceneManager.LoadScene(Utilities.SceneNames.JoinMatch.ToString());
        }
        else if(SceneManager.GetActiveScene().name == Utilities.SceneNames.JoinMatch.ToString())
        {
            SessionManager.instance.ConnectServer();
        }
        else if (SceneManager.GetActiveScene().name == Utilities.SceneNames.MatchMaking.ToString())
        {
            Debug.Log("Match making loaded");
        }
    }

    

#endif
}
