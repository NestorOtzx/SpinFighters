using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchSettings : NetworkBehaviour
{
    private void Start()
    {
        GameManager.instance.RestartMatch();

    }
}
