using System.Collections;
using System.Collections.Generic;
using TankStatistics;
using UnityEngine;
using TMPro;

public class StatScreen : MonoBehaviour
{
    public static StatScreen instance;
    public TextMeshProUGUI stats;

    public GameObject StatPanel;
    public Transform StatPanelContainer;
    List<GameObject> statPanels = new List<GameObject>();

    List<Stats> playerStats = new List<Stats>();
    List<Client> clients = new List<Client>();
    Stats match = new Stats();

    Dictionary<string, Stats> openWith = new Dictionary<string, Stats>();

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void PassStats(List<Stats> playerStats, Stats match, List<Client> clients)
    {
        this.match = match;
        this.playerStats = playerStats;

        Client temp;

        for (int i = 1; i < clients.Count; i++)
        {
            int j = i;
            while (clients[j - 1].score < clients[j].score)
            {
                temp = clients[j - 1];
                clients[j - 1] = clients[j];
                clients[j] = temp;
                if (j != 1)
                {
                    j--;
                }
            }
        }

        this.clients = clients;

        SetStats();
    }

    private void SetStats()
    {
        stats.text = $"total shots: {match.shots}, airdrops picked up: {match.ADTotal}";
        for(int i = 0; i < clients.Count; i++)
        {
            StatPanel panel = Instantiate(StatPanel, StatPanelContainer).GetComponent<StatPanel>();
            statPanels.Add(panel.gameObject);
            panel.SetUsername(clients[i].username);
            panel.SetScore(clients[i].score);
            panel.SetPlacementIcon(i + 1);

            //stats
            panel.SetCommonStats(playerStats[i].kills, playerStats[i].shots, playerStats[i].closeCalls, playerStats[i].ADTotal);
            panel.SetADStats(playerStats[i]);
        }
    }
}
