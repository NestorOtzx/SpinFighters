using Unity.Netcode;
using UnityEngine;

public class InstantiateOnCollision : NetworkBehaviour
{
    public GameObject prefabToInstantiate; // Prefab a instanciar
    public LayerMask collisionLayer;      // Capa de colisión a detectar

    private void OnCollisionEnter(Collision collision)
    {
        if ((collisionLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            if (GameManager.instance.isSinglePlayer)
            {
                CreateCollsion(collision.contacts[0].point);
            }else if (NetworkManager.Singleton.IsServer){
                CreateCollsionClientRpc(collision.contacts[0].point);
            }
            
        }   
    }

    [ClientRpc]
    private void CreateCollsionClientRpc(Vector3 point)
    {
        CreateCollsion(point);
    }

    private void CreateCollsion(Vector3 point)
    {
        Vector3 spawnPosition = point;
        Instantiate(prefabToInstantiate, spawnPosition, Quaternion.Euler(new Vector3(-90, 0, 0)));
    }
}