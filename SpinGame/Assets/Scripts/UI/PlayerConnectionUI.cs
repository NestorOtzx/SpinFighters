using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConnectionUI : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    private Dictionary<ulong, GameObject> instances = new Dictionary<ulong, GameObject>();

    public bool showScores = true;
    void Start()
    {
        StartCoroutine(LateStart());
        PlayerConnection.instance.OnClientsUpdate += OnListUpdate;
    }

    private void OnDisable()
    {
        PlayerConnection.instance.OnClientsUpdate -= OnListUpdate;
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
        OnListUpdate();
    }

    public void OnListUpdate()
    {
        ClearAll();
        if (GameManager.instance.isSinglePlayer)
        {
            for (int i = 0; i<PlayerConnection.instance.clientInfoSingle.Count; i++)
            {
                InstantiateBlock(PlayerConnection.instance.clientInfoSingle[i]);
            }
        }else{
            for (int i = 0; i<PlayerConnection.instance.clientInfo.Count; i++)
            {
                InstantiateBlock(PlayerConnection.instance.clientInfo[i]);
            }

        }     
    }


    private void InstantiateBlock(ConnectionData playerData)
    {
        GameObject obj = Instantiate(blockPrefab, transform);
        PlayerConnectionBlock block = obj.GetComponent<PlayerConnectionBlock>();
        block.SetScore(showScores);
        block.SetValues(playerData.username.ToString(), playerData.isHost, playerData.score);
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
