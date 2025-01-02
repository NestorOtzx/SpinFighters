using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioOnCollsion : MonoBehaviour
{
    public int audioID;
    public LayerMask collisionLayer;

    private void OnCollisionEnter(Collision collision)
    {
        if ((collisionLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            if (GameManager.instance.isSinglePlayer)
            {
                CreateAudio(collision.contacts[0].point);
            }else if (NetworkManager.Singleton.IsServer){
                CreateAudioClientRpc(collision.contacts[0].point);
            }
            
        }   
    }

    [ClientRpc]
    private void CreateAudioClientRpc(Vector3 point)
    {
        CreateAudio(point);
    }

    private void CreateAudio(Vector3 point)
    {
        AudioManager.instance.PlayAudio(audioID, point);
    }
}
