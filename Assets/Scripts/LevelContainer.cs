using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelContainer : MonoBehaviour
{
    public List<GameObject> AllLevels = new List<GameObject>();
    public List<GameObject> LevelsFor2 = new List<GameObject>();
    public List<GameObject> LevelsFor3 = new List<GameObject>();
    public List<GameObject> LevelsFor4 = new List<GameObject>();

    private void Awake()
    {
        foreach (GameObject lvlgo in AllLevels)
        {
            Level lvl = lvlgo.GetComponent<Level>();
            if(lvl.capacity == 2)
            {
                LevelsFor2.Add(lvlgo);
            }
            if(lvl.capacity == 3)
            {
                LevelsFor2.Add(lvlgo);
                LevelsFor3.Add(lvlgo);
            }
            if(lvl.capacity == 4)
            {
                LevelsFor2.Add(lvlgo);
                LevelsFor3.Add(lvlgo);
                LevelsFor4.Add(lvlgo);
            }
        }
    }
}
