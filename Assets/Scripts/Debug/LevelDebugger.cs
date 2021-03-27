using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDebugger : MonoBehaviour
{
    LevelConfig LevelConfig;
    public List<GameObject> playerUISpawnPoints = new List<GameObject>();
    public void StartServer()
    {
        Server.Start(4, 26950);
    }

    private void Start()
    {
        StartServer();
        LevelConfig = GameObject.FindGameObjectWithTag("LevelConfig").GetComponent<LevelConfig>();
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
}
