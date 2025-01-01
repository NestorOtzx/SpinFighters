using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class PortPrinter : MonoBehaviour
{
    [SerializeField] private string prefix;
    [SerializeField] private TextMeshProUGUI portText;
    [SerializeField] private TextMeshProUGUI portShadow;
    void Start()
    {
        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
        string port = prefix+NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port.ToString();
        portText.text = port;
        portShadow.text = port;
    }
}
