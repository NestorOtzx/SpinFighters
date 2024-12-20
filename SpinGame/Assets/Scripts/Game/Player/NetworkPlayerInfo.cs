using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class NetworkPlayerInfo : NetworkBehaviour
{
    private int instanceID;


    public int GetPlayerInstanceID(){
        return instanceID;
    }
    
    public void SetPlayerInstanceID(int newID)
    {
        instanceID = newID;
    }

    private void Start()
    {
        GameManager.instance.OnSpawnNewPlayer(this);
        gameObject.name = "Player "+(instanceID).ToString();
    }





}
