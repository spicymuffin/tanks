using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public SceneField scene;
    public GameObject icon;
    public int capacity;

    public Level(GameObject _icon, SceneField _scene)
    {
        icon = _icon;
        scene = _scene;
    }

    public void AddToMatch()
    {
        MatchMaker mmkr = GameObject.FindGameObjectWithTag("MatchMaker").GetComponent<MatchMaker>();
        mmkr.AddToQueue(this);
    }
}
