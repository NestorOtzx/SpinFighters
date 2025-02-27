using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class DeathZone : NetworkBehaviour
{
    public LayerMask playerMask;

    private List<Rigidbody> objectsTriggered = new List<Rigidbody>();

    public bool useConstantSpeed = true;
    [SerializeField] private float speed = 1;

    private void OnTriggerEnter(Collider other)
    {
        if ((!GameManager.instance ||GameManager.instance.isSinglePlayer) || NetworkManager.Singleton.IsServer)
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            if (!rb)
                rb = other.gameObject.GetComponentInParent<Rigidbody>();
            if (rb)
            {
                if (useConstantSpeed)
                {
                    rb.useGravity = false;
                    rb.angularVelocity = Vector3.zero;
                    rb.velocity = new Vector3(0, -speed, 0);
                }
            }

            if (((1 << other.gameObject.layer) & playerMask) != 0){
                ulong id = other.gameObject.GetComponentInParent<PlayerInfo>().playerID;
                GameManager.instance.SetLooserSrv(id);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((!GameManager.instance ||GameManager.instance.isSinglePlayer) || NetworkManager.Singleton.IsServer)
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            if (!rb)
                rb = other.gameObject.GetComponentInParent<Rigidbody>();
            if (rb)
            {
                rb.useGravity = false;
                rb.angularVelocity = Vector3.zero;
                rb.velocity = new Vector3(0, 0, 0);
                rb.isKinematic = true;
            }

            if (((1 << other.gameObject.layer) & playerMask) != 0){
                ulong id = other.gameObject.GetComponentInParent<PlayerInfo>().playerID;
                GameManager.instance.SetLooserSrv(id);
            }
        }
    }
}
