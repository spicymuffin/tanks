using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDebugger : MonoBehaviour
{
    LevelConfig LevelConfig;
    AirdropManager AirdropManager;
    public List<GameObject> playerUISpawnPoints = new List<GameObject>();
    public void StartServer()
    {
        Server.Start(4, 26950);
    }

    private void Start()
    {
        StartServer();
        LevelConfig = GameObject.FindGameObjectWithTag("LevelConfig").GetComponent<LevelConfig>();
        AirdropManager = GameObject.FindGameObjectWithTag("AirdropManager").GetComponent<AirdropManager>();
        if (LevelConfig == null)
        {
            Debug.LogWarning("LevelConfig not found");
        }
        if (AirdropManager == null)
        {
            Debug.LogWarning("AirdropManager not found");
        }
    }

    public void Spawn()
    {
        LevelConfig.RandomizeSpawnPoints();
        int index = 0;
        List<Vector3> spawnpoints = LevelConfig.spawnpoints;
        foreach (Client _client in Server.clients.Values)
        {
            if (_client.connected == true && _client.player == null)
            {
                _client.SendIntoGame(spawnpoints[index]);
                _client.player.StartUpUI(playerUISpawnPoints[index]);
                _client.player.myClient = _client;
                index++;
            }
        }
    }

    public void SpawnAD()
    {
        AirdropManager.SpawnRndAirdrop();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        ServerSend.DisconnectAll();
    }
}
