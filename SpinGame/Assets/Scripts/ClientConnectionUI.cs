using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;


public class ClientConnectionUI : NetworkBehaviour
{
    [SerializeField] private Button [] buttons;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;
    
    private void Start()
    {
        buttons[0].onClick.AddListener(() => {
            Debug.Log("Clicked Find games");
            });
        buttons[1].onClick.AddListener(() => {
            Debug.Log("Clicked connect player");
            SessionManager.instance.ConnectClientToMatch(ipInput.text,  ushort.Parse(portInput.text));
            });
        buttons[2].onClick.AddListener(() => {
            Debug.Log("Clicked create match");
            SessionManager.instance.CreateMatch(ipInput.text);
            });
    }
}
