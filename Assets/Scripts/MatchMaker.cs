using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class MatchMaker : MonoBehaviour
{
    public static MatchMaker instance;
    private LevelContainer LC;
    public GameObject LevelSelectionPanel;
    public List<GameObject> selection;
    public GameObject MatchQueueIcons;
    public List<GameObject> icons = new List<GameObject>();
    public Button StartButton;

    public TextMeshProUGUI counterTextField;
    int counter = 1;

    public List<Level> match = new List<Level>();

    public void Awake()
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

    private void Start()
    {
        LC = FindObjectOfType<LevelContainer>();
    }

    public void LoadLevels(int _playerCount)
    {
        if (_playerCount == 2)
        {
            selection = LC.LevelsFor2;
        }
        else if (_playerCount == 3)
        {
            selection = LC.LevelsFor3;
        }
        else if (_playerCount == 4)
        {
            selection = LC.LevelsFor4;
        }
        else
        {
            selection = LC.LevelsFor4;
            //Debug.LogError($"Player count mismatch, {_playerCount} connected. Entering debug mode");
        }
        foreach(GameObject lvl in selection)
        {
            Instantiate(lvl, LevelSelectionPanel.transform);
        }
    }


    public void ResetMatchMaker()
    {
        MainMenu.instance.MoveToMainMenu();
        selection.Clear();
        for(int i = 0; i < icons.Count; i++)
        {
            Destroy(icons[i]);
        }
        icons.Clear();
        match.Clear();
        SetCounter(1);
        CreateGameScreen.instance.ResetWaitScreen();
        Server.Stop();
    }

    public void AddToQueue(Level _toAdd)
    {
        if (match.Count != 8)
        {
            match.Add(_toAdd);
            GameObject icon = Instantiate(GetLastLevel().icon, MatchQueueIcons.transform);
            icon.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            icons.Add(icon);
            StartButton.interactable = true;
        }

    }
    public void RemoveFromQueue()
    {
        if (match.Count != 0)
        {
            Destroy(GetLastIcon());
            match.RemoveAt(match.Count - 1);
            icons.RemoveAt(icons.Count - 1);
        }
        if (match.Count == 0)
        {
            StartButton.interactable = false;
        }
    }
    public void PassQueue()
    {
        FindObjectOfType<GameManager>().SetQueue(match);
    }
    public Level GetLastLevel()
    {
        return match[match.Count - 1];
    }
    public GameObject GetLastIcon()
    {
        return icons[icons.Count - 1];
    }
    public void AddCounter()
    {
        if (counter != 8) counter++;
        counterTextField.text = counter.ToString();
    }
    public void SubtractCounter()
    {
        if (counter != 1) counter--;
        counterTextField.text = counter.ToString();
    }
    public void SetCounter(int _counter)
    {
        counter = _counter;
        counterTextField.text = counter.ToString();
    }
}
