using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerConnectionBlock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI username;

    public void SetValues(string username)
    {
        this.username.text = username;
    }


}
