using UnityEngine;
using Unity.Netcode;

public class ObjectSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject [] objectsToSpawn;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float spawnInterval = 2f;

    private float timer;

    private void Update()
    {
        if ((!GameManager.instance || GameManager.instance.isSinglePlayer) || (NetworkManager.Singleton && NetworkManager.Singleton.IsServer))
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer = 0f;
                SpawnObject();
            }
        }
    }

    private void SpawnObject()
    {
        Vector3 randomPosition = GetRandomPositionWithinRadius();
        if (!GameManager.instance || GameManager.instance.isSinglePlayer)
        {
            int objId = Random.Range(0, objectsToSpawn.Length);
            Debug.Log(objId);
            var obj = Instantiate(objectsToSpawn[objId], randomPosition, Quaternion.identity);
            obj.transform.localScale = Vector3.one*Random.Range(0.5f, 2f)*100;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = false;
            }
        }
        else if (IsServer)
        {
            SpawnObjectServerRpc(randomPosition);
        }
    }

    private Vector3 GetRandomPositionWithinRadius()
    {
        Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
        return new Vector3(randomPoint.x, transform.position.y, randomPoint.y);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnObjectServerRpc(Vector3 position)
    {
        GameObject spawnedObject = Instantiate(objectsToSpawn[Random.Range(0, objectsToSpawn.Length)], position, Quaternion.identity);
        spawnedObject.transform.localScale = Vector3.one*Random.Range(0.5f, 2f)*100;
        spawnedObject.GetComponent<NetworkObject>().Spawn();
    }
}
