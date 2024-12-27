using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Session{
    public int port {get; set;}
    public string name {get; set;}
    public int pid {get; set;}
    public Session(string _name, int _port, int _pid)
    {
        name =_name; port = _port; pid = _pid;
    }
}

public class SessionFinder : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform blocksContainer;
    [SerializeField] private GameObject blockPrefab;

    [SerializeField] private TMP_InputField serverIp;
    [SerializeField] private TMP_InputField username;
    List<GameObject> prefInstances = new List<GameObject>();
    public int currentPort;

    SessionManager manager;
    void Start()
    {
        manager = FindObjectOfType<SessionManager>();
        currentPort = -1;
        //FindMatches();
    }

    public void ConnectToSelected()
    {
        manager.ConnectClientToMatch(serverIp.text, (ushort)currentPort, username.text);
    }

    public void FindMatches()
    {
        StartCoroutine(FindMatchesCoroutine(serverIp.text));
    }

    IEnumerator FindMatchesCoroutine(string ip)
    {
        string url = "http://"+ip+":5100/MatchMaking/GetMatches";
        Debug.Log(url);

        // Crear la solicitud
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Enviar la solicitud y esperar la respuesta
            yield return webRequest.SendWebRequest();

            // Manejo de errores
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                List<Session> sessions = JsonConvert.DeserializeObject<List<Session>>(jsonResponse);

                foreach(var obj in prefInstances)
                {
                    Destroy(obj);
                }
                prefInstances.Clear();

                foreach (Session s in sessions)
                {
                    GameObject obj = Instantiate(blockPrefab, blocksContainer);
                    SessionBlock block = obj.GetComponent<SessionBlock>();
                    block.SetText(s.name, s.port, "0/10");
                    prefInstances.Add(obj);
                }
                Debug.Log("Current matches: "+ jsonResponse);
            }
        }
    }
}
