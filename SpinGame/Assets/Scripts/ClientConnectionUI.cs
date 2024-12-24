using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;


public class ClientConnectionUI : MonoBehaviour
{
    [SerializeField] private Button [] buttons;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;
    
    private void Start()
    {
        SessionManager session =FindObjectOfType<SessionManager>();
        buttons[0].onClick.AddListener(() => {
            Debug.Log("Clicked Find games");
            });
        buttons[1].onClick.AddListener(() => {
            Debug.Log("Clicked connect player");
            session.ConnectClientToMatch(ipInput.text,  ushort.Parse(portInput.text));
            });
        buttons[2].onClick.AddListener(() => {
            Debug.Log("Clicked create match");
            session.CreateMatch(ipInput.text);
            });
    }
}
