using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerConnectionBlock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI username, hostText, score;

    public void SetScore(bool b)
    {
        score.gameObject.SetActive(b);
    }

    public void SetValues(string username, bool isHost, int score)
    {
        this.username.text = username;
        this.hostText.gameObject.SetActive(isHost);
        this.score.text = "Score: "+score.ToString();
    }
}
