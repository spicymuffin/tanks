using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TankStatistics;

public class StatPanel : MonoBehaviour
{
    public List<GameObject> placementIcons = new List<GameObject>();
    public List<string> placementStrings = new List<string>();
    public TextMeshProUGUI username;
    public TextMeshProUGUI common;
    public TextMeshProUGUI adspecific;
    public TextMeshProUGUI score;
    public TextMeshProUGUI rank;

    public void SetCommonStats(int kills, int shots, int closeCalls, int ADTotal)
    {
        common.text = $"kills: {kills}\nshots fired: {shots}\nbullets dodged: " +
            $"{closeCalls}\n airdrops picked up: {ADTotal}";
    }

    public void SetUsername(string username)
    {
        this.username.text = username;
    }

    public void SetScore(int score)
    {
        this.score.text = score.ToString();
    }

    public void SetPlacementIcon(int place)
    {
        placementIcons[place - 1].SetActive(true);
        rank.text = placementStrings[place - 1];
    }

    public void SetADStats(Stats stat)
    {
        if (stat.shieldBlocks != 0)
        {
            adspecific.text += $"bullets blocked by shield: {stat.shieldBlocks}\n";
        }
        if (stat.landminesCreated != 0)
        {
            adspecific.text += $"bullets blocked by shield: {stat.shieldBlocks}\n";
        }
        if (stat.landmineKills != 0)
        {
            adspecific.text += $"bullets blocked by shield: {stat.shieldBlocks}\n";
        }
    }
}
