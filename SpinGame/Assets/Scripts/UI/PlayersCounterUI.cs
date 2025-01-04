using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayersCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playersCounter_tm;
    [SerializeField] private TextMeshProUGUI shadow_tm;

    private void FixedUpdate()
    {
        if (NetworkManager.Singleton)
        {
            int playerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            string text = "Players: "+playerCount.ToString()+"/8";
            playersCounter_tm.text = text;
            shadow_tm.text = text;
        }
    }
}
