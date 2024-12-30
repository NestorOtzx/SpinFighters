using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinnerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameText;
    void Start()
    {
        usernameText.text = "";
        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
        ConnectionData winner = PlayerConnection.instance.GetWinner();
        usernameText.text = winner.username.ToString()+"!";
    }
}
