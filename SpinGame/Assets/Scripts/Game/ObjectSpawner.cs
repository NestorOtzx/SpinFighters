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
        
        if (!GameManager.instance || GameManager.instance.isSinglePlayer)
        {
            var obj = SpawnRandomObject();
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = false;
            }
        }
        else if (IsServer)
        {
            var obj = SpawnRandomObject();
            obj.GetComponent<NetworkObject>().Spawn(true);
        }
    }

    private Vector3 GetRandomPositionWithinRadius()
    {
        Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
        return new Vector3(randomPoint.x, transform.position.y, randomPoint.y);
    }

    private GameObject SpawnRandomObject()
    {
        Vector3 randomPosition = GetRandomPositionWithinRadius();
        int objId = Random.Range(0, objectsToSpawn.Length);
        var obj = Instantiate(objectsToSpawn[objId], randomPosition, Quaternion.identity);
        obj.transform.localScale = Vector3.one*Random.Range(0.5f, 2f)*100;
        return obj;
    }
}
