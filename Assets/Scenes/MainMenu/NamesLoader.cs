using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class NamesLoader : MonoBehaviour
{
    public static NamesLoader instance;

    public TextAsset sess_nm_file;
    public TextAsset player_nm_file;

    public string[] sess_names;
    public string[] player_names;


    private void Awake()
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
    public void Start()
    {
        LoadFiles();
    }

    public string GetRandomString(string[] list)
    {
        System.Random random = new System.Random();
        int index = random.Next(list.Length);
        return list[index];
    }
    public void LoadFiles()
    {
        player_names = ReadTextFile(player_nm_file);
        sess_names = ReadTextFile(sess_nm_file);
    }
    public string[] ReadTextFile(TextAsset file)
    {
        return file.text.Split('\n');
    }
}
