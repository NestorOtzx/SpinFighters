using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button [] buttons;

    private void Awake()
    {
        buttons[0].onClick.AddListener(() => {NetworkManager.Singleton.StartServer();});
        buttons[1].onClick.AddListener(() => {NetworkManager.Singleton.StartHost();});
        buttons[2].onClick.AddListener(() => {NetworkManager.Singleton.StartClient();});
    }

}
