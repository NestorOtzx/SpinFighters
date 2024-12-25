using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerInitializer : MonoBehaviour
{
    public GameObject [] rigidBodys;
    void Start()
    {
        if (GameManager.instance.isSinglePlayer)
        {
            for (int i=0; i<rigidBodys.Length; i++)
            {
                rigidBodys[i].GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

}
