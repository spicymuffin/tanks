using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalStatsManager : MonoBehaviour
{
    public int totalShots;
    public int totalKills;
    public int totalDeaths;
    public Text shots;
    public Text kills;
    public Text deaths;
    public void ShowStats()
    {
        totalShots = PlayerPrefs.GetInt("totalShots");
        totalKills = PlayerPrefs.GetInt("totalKills");
        totalDeaths = PlayerPrefs.GetInt("totalDeaths");
        shots.text = totalShots.ToString();
        kills.text = totalKills.ToString();
        deaths.text = totalDeaths.ToString();

    }
}
