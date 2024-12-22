using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSettings : MonoBehaviour
{
    private void Awake()
    {
        GameManager.instance.RestartMatch();

    }
}
