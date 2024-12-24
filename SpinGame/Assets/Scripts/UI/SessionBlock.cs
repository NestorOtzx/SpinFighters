using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionBlock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI portText;
    [SerializeField] private TextMeshProUGUI playersText;

    private Button button;

    SessionFinder finder;

    int currentPort = -1;

    private void Start()
    {
        button = GetComponent<Button>();
        finder = FindObjectOfType<SessionFinder>();

        button.onClick.AddListener(()=> {
            if (currentPort > 0)
            {
                Debug.Log("Port selected: "+ currentPort);
                finder.currentPort = currentPort;
            }
        });
    }

    public void SetText(string name, int port, string players)
    {
        nameText.text = name;
        portText.text = port.ToString();
        playersText.text = players;
        currentPort = port;
    }
}
