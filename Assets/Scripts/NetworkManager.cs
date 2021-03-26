using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        //Server.Start(50, 26950);
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity).GetComponent<Player>();
    }

    public Player InstantiatePlayer(Vector3 _position)
    {
        return Instantiate(playerPrefab, _position, Quaternion.identity).GetComponent<Player>();
    }
}
