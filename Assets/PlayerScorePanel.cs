using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScorePanel : MonoBehaviour
{
    public TextMeshProUGUI username;
    public TextMeshProUGUI score;

    public void SetScore(int _score)
    {
        this.score.text = _score.ToString();
    }

    public void SetUsername(string _name)
    {
        username.text = _name;
    }
}
