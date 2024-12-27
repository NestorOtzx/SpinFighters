using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using UnityEngine.SceneManagement;


public class ClientConnectionUI : MonoBehaviour
{
    [SerializeField] private Button [] buttons;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;
    [SerializeField] private TMP_InputField usernameInput;
    
    private void Start()
    {
        SessionManager session =FindObjectOfType<SessionManager>();
        buttons[0].onClick.AddListener(() => {
            Debug.Log("Clicked Find games");
            SceneManager.LoadScene(Utilities.SceneNames.FindMatch.ToString());
            });
        buttons[1].onClick.AddListener(() => {
            Debug.Log("Clicked connect player");
            session.ConnectClientToMatch(ipInput.text,  ushort.Parse(portInput.text), usernameInput.text);
            });
        buttons[2].onClick.AddListener(() => {
            Debug.Log("Clicked create match");
            session.CreateMatch(ipInput.text, usernameInput.text);
            });
    }
}
