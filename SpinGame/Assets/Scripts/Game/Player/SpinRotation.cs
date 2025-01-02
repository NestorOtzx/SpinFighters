using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpinRotation : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public void Start()
    {
        if (GameManager.instance.isSinglePlayer)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));   
        }
        else if (NetworkManager.Singleton.IsServer)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
            SetStartRotationClientRpc();
        }
    }

    [ClientRpc]
    private void SetStartRotationClientRpc()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
    }



    void Update()
    {
        float rotationAngle = rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotationAngle, 0);
        
    }
}
