using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalStatsManager : MonoBehaviour
{
    public int totalShots;
    public Text shots;
    public void ShowStats()
    {
        totalShots = PlayerPrefs.GetInt("totalShots");
        shots.text = totalShots.ToString();

    }
}
