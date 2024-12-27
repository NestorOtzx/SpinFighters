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
    [SerializeField] TextMeshProUGUI botsText;

    private void Start()
    {
        roundsText.text = GameManager.instance.numberOfRounds.ToString();
        increaseRounds.onClick.AddListener(() => AddRounds(2));
        decreaseRounds.onClick.AddListener(() => AddRounds(-2));

        botsText.text = GameManager.instance.numberOfRounds.ToString();
        increaseBots.onClick.AddListener(() => AddBots(1));
        decreaseBots.onClick.AddListener(() => AddBots(-1));
    }

    private void FixedUpdate()
    {
        roundsText.text = GameManager.instance.numberOfRounds.ToString();
        botsText.text = GameManager.instance.botsNumber.ToString();
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
        int newValue = (int)GameManager.instance.botsNumber+value;
        if (newValue > 0 && newValue < 8){
            GameManager.instance.SetNumberOfBots((uint)newValue);
            
        }
    }
}
