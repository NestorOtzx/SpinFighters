using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SessionBlock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI portText;
    [SerializeField] private TextMeshProUGUI playersText;

    public void SetText(string name, string port, string players)
    {
        nameText.text = name;
        portText.text = port;
        playersText.text = players;
    }
}
