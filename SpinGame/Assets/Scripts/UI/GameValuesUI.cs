using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameValuesUI : MonoBehaviour
{
    [SerializeField] Button increaseRounds, decreaseRounds;
    [SerializeField] TextMeshProUGUI roundsText;

    [SerializeField] Button increaseBots, decreaseBots;

    private void Start()
    {
        roundsText.text = GameManager.instance.numberOfRounds.ToString();
        increaseRounds.onClick.AddListener(() => AddRounds(1));
        decreaseRounds.onClick.AddListener(() => AddRounds(-1));

        increaseBots.onClick.AddListener(() => AddBots(1));
        decreaseBots.onClick.AddListener(() => AddBots(-1));
    }

    private void FixedUpdate()
    {
        roundsText.text = GameManager.instance.numberOfRounds.ToString();
    }

    public void AddRounds(int value)
    {
        int newValue = GameManager.instance.numberOfRounds+value;
        if (newValue > 0 && newValue < 10){
            GameManager.instance.SetNumberOfRounds(newValue);
        }
    }

    public void AddBots(int value)
    {
        int clientsCount = PlayerConnection.instance.clientInfoSingle.Count;
        int newValue = (int)clientsCount+value;
        if (newValue > 0 && newValue <= 8){
        
            if (value > 0)
            {
                PlayerConnection.instance.ConnectSinglePlayer("Bot "+clientsCount.ToString(), 0, false);
            }else{  
                PlayerConnection.instance.DisconnectSinglePlayer((ulong)(clientsCount-1));
            }
            
        }
    }
}
