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
    private static bool runAsServer = false;
    void Start()
    {
        if (runAsServer)
        {
            RunServer();
        }
#if UNITY_SERVER
        if (!Application.isEditor)
        {
            RunServer();
        }
#endif
    }

    public void RunServer()
    {
        runAsServer = true;

        if (SceneManager.GetActiveScene().name == Utilities.SceneNames.MainMenu.ToString())
        {
            SceneManager.LoadScene(Utilities.SceneNames.JoinMatch.ToString());
        }
        else if(SceneManager.GetActiveScene().name == Utilities.SceneNames.JoinMatch.ToString())
        {
            FindObjectOfType<SessionManager>().ConnectServer();
        }
        else if (SceneManager.GetActiveScene().name == Utilities.SceneNames.MatchMaking.ToString())
        {
            Debug.Log("Match making loaded");
        }
    }


}
