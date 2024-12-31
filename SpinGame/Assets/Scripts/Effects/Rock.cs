using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] private ParticleSystem fireParticles, fallParticles;

    private void OnCollisionEnter(Collision other)
    {
        fallParticles.Stop();
        StartCoroutine(StopFireParticles());
    }

    private void OnTriggerEnter(Collider other)
    {
        fallParticles.Play();
    }

    private IEnumerator StopFireParticles()
    {
        yield return new WaitForSeconds(10f);
        if (gameObject)
        {
            fireParticles.Stop(true);
        }
    }

    private void FixedUpdate()
    {
        if ((!GameManager.instance || GameManager.instance.isSinglePlayer || NetworkManager.Singleton.IsServer) && transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

}
