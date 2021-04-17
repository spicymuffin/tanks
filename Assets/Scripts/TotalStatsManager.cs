using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TotalStatsManager : MonoBehaviour
{
    public int shots;
    public int kills;
    public int deaths;
    public TextMeshProUGUI shotsField;
    public TextMeshProUGUI killsField;
    public TextMeshProUGUI deathsField;
    private void Awake()
    {
        shots = PlayerPrefs.GetInt("totalShots");
        kills = PlayerPrefs.GetInt("totalKills");
        deaths = PlayerPrefs.GetInt("totalDeaths");
        shotsField.text = $"total shots: {shots}";
        killsField.text = $"total kills: {kills}";
        deathsField.text = $"total deaths: {deaths}";
    }
}
