using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConnectionStatus : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI errorText;

    [SerializeField] private GameObject loadingIcon;
    private SessionManager sessionManager;
    // Start is called before the first frame update
    void Start()
    {
        sessionManager = FindObjectOfType<SessionManager>();
        sessionManager.StateMessageEvents += OnDisconnect;
        sessionManager.onClientStartConnection += StartingToConnect;
        sessionManager.onClientStopConnection += FinishToConnect;
        errorText.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        sessionManager.StateMessageEvents -= OnDisconnect;
        sessionManager.onClientStartConnection -= StartingToConnect;
        sessionManager.onClientStopConnection -= FinishToConnect; 
    }

    private void OnDisconnect(string reason)
    {
        loadingIcon.SetActive(false);
        errorText.gameObject.SetActive(true);
        errorText.text = reason;
    }

    private void StartingToConnect()
    {
        errorText.gameObject.SetActive(false);
        loadingIcon.SetActive(true);
    }

    private void FinishToConnect()
    {
        loadingIcon.SetActive(false);
    }
}
