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
        Debug.Log("Collision enter");
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
            Debug.Log("Collision enter is server" + other.gameObject.name);

            if (((1 << other.gameObject.layer) & playerMask) != 0){
                Debug.Log("Collision enter is mask");
                ulong id = other.gameObject.GetComponentInParent<PlayerInfo>().playerID;
                GameManager.instance.SetLooserSrv(id);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Collision enter");
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
            Debug.Log("Collision enter is server" + other.gameObject.name);

            if (((1 << other.gameObject.layer) & playerMask) != 0){
                Debug.Log("Collision enter is mask");
                ulong id = other.gameObject.GetComponentInParent<PlayerInfo>().playerID;
                GameManager.instance.SetLooserSrv(id);
            }
        }
    }
}
