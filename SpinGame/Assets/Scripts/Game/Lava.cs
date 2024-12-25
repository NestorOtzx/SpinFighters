using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Lava : NetworkBehaviour
{
    public LayerMask playerMask;
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision enter");
        if (GameManager.instance.isSinglePlayer || NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Collision enter is server" + other.gameObject.name);

            if (((1 << other.gameObject.layer) & playerMask) != 0){
                Debug.Log("Collision enter is mask");
                ulong id = other.gameObject.GetComponent<PlayerInfo>().playerID;
                GameManager.instance.SetLooserSrv(id);
            }
        }
    }
}
