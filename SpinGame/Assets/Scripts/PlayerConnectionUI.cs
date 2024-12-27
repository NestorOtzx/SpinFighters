using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConnectionUI : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    private Dictionary<ulong, GameObject> instances = new Dictionary<ulong, GameObject>();
    void Start()
    {
        OnListUpdate();
    }

    public void OnListUpdate()
    {
        ClearAll();
        
        for (int i = 0; i<PlayerConnection.instance.clientInfo.Count; i++)
        {
            InstantiateBlock(PlayerConnection.instance.clientInfo[i]);
        }
    }


    private void InstantiateBlock(ConnectionData playerData)
    {
        GameObject obj = Instantiate(blockPrefab, transform);
        PlayerConnectionBlock block = obj.GetComponent<PlayerConnectionBlock>();
        block.SetValues(playerData.username.ToString());
        instances.Add(playerData.clientID, obj);
    }

    private void ClearAll()
    {
        foreach(var inst in instances.Keys)
        {
            Destroy(instances[inst].gameObject);
        }
        instances.Clear();
    }

}
