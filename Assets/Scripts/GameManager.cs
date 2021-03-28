using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankStatistics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    List<Level> queue;
    LevelConfig LevelConfig;

    List<Client> clients = new List<Client>();
    List<Player> playersAlive = new List<Player>();
    List<Player> players4datacollect = new List<Player>();
    List<RoundStats> matchStats = new List<RoundStats>();
    List<PlayerScorePanel> scoreInterfaces = new List<PlayerScorePanel>();
    List<Stats> playerStats = new List<Stats>();
    Stats match = new Stats();

    [Space(30)]
    public GameObject ScoreboardUI;
    public List<GameObject> scoreboardPanelSpawnPoints = new List<GameObject>();

    [Space(30)]
    public GameObject PlayerUI;
    public List<GameObject> playerUISpawnPoints = new List<GameObject>();

    [Header("Prefabs")]
    public GameObject ScoreBoardPanelPrefab;

    bool isLoading = false;

    private bool lastRound = false;
    public SceneField winScene;

    public class RoundStats
    {
        GameManager GameManager;
        //for round-summary
        public List<Stats> playerStats = new List<Stats>();

        public void SetPlayerStats(int _id, Stats _stats)
        {
            playerStats[_id - 1] = _stats;
            //PrintStatistic(_stats);
        }
        public void PrintStatistic(Stats _stats)
        {
            Debug.Log($"displaying stats for: {_stats.client.username}");
            Debug.Log($"shots: {_stats.shots}");
            Debug.Log($"closeCalls: {_stats.closeCalls}");
            Debug.Log($"ADTotal: {_stats.ADTotal}");
            Debug.Log($"kills: {_stats.kills}");
            Debug.Log($"deaths: {_stats.deaths}");
            Debug.Log($"shieldBlocks: {_stats.shieldBlocks}");
            Debug.Log($"landminesCreated: {_stats.landminesCreated}");
            Debug.Log($"landmineKills: {_stats.landmineKills}");
        }
        public void CollectPlayerStats()
        {
            foreach (Client _client in GameManager.clients)
            {
                Debug.Log($"collecting stats for player: {_client.username}");
                _client.player.SendStats();
            }
        }
        public RoundStats(GameManager _GameManager)
        {
            GameManager = _GameManager;
            foreach (Client _client in GameManager.clients)
            {
                if (_client.connected == true)
                {
                    playerStats.Add(new Stats());
                }
            }
        }
    }
    public RoundStats currentRoundStats;

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
        //fucking slaves
        DontDestroyOnLoad(this.gameObject);
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        ServerSend.DisconnectAll();
    }

    public void ResetServer()
    {
        Server.Stop();
    }

    #region UI
    public void ClearPlayerUI()
    {
        foreach (GameObject panel in playerUISpawnPoints)
        {
            try
            {
                Destroy(panel.transform.GetChild(0).gameObject);
            }
            catch
            {

            }
        }
    }
    public void StartScoreboardUI()
    {
        foreach (Client client in clients)
        {
            GameObject currentPanel = Instantiate(ScoreBoardPanelPrefab, scoreboardPanelSpawnPoints[client.id - 1].transform);
            PlayerScorePanel panelInterface = currentPanel.GetComponent<PlayerScorePanel>();
            panelInterface.SetUsername(client.username);
            panelInterface.SetScore(0);
            scoreInterfaces.Add(panelInterface);
        }
    }
    #endregion
    #region Match
    public void StartMatch()
    {
        CreateClientsList();
        StartScoreboardUI();
        if (queue.Count == 1)
        {
            lastRound = true;
        }
        StartCoroutine(LoadAndStartFirstLevel(queue[0].scene));
    }
    IEnumerator LoadAndStartFirstLevel(SceneField _scene)
    {
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(_scene, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            print("Loading the Scene");
            yield return null;
        }
        LoadLevelData();
        queue.RemoveAt(0);
        StartRound();
    }
    #endregion
    #region Round-time
    public void KillPlayer(Player _player)
    {
        playersAlive.Remove(_player);
        Check4EndRound();
    }
    public void StartRound()
    {
        currentRoundStats = new RoundStats(this);
        Spawn();
        PlayerUI.SetActive(true);
    }
    public void EndRound(Client winner)
    {
        currentRoundStats.CollectPlayerStats();
        matchStats.Add(currentRoundStats);
        winner.score++;
        scoreInterfaces[winner.id - 1].SetScore(winner.score);
        StartCoroutine(ShowRoundEndScreen());
    }
    public void LoadLevelData()
    {
        LevelConfig = GameObject.FindGameObjectWithTag("LevelConfig").GetComponent<LevelConfig>();
    }
    public void Check4EndRound()
    {
        Debug.Log($"players alive:{playersAlive.Count}");
        if (playersAlive.Count == 1)
        {
            EndRound(playersAlive[0].myClient);
            playersAlive[0].StopAllCoroutines();
            playersAlive.Clear();
        }
    }
    public void Spawn()
    {
        LevelConfig.RandomizeSpawnPoints();
        int index = 0;
        List<Vector3> spawnpoints = LevelConfig.spawnpoints;
        foreach (Client _client in Server.clients.Values)
        {
            if (_client.connected == true && _client.player == null)
            {
                _client.SendIntoGame(spawnpoints[index]);
                _client.player.StartUpUI(playerUISpawnPoints[index]);
                _client.player.myClient = _client;
                playersAlive.Add(_client.player);
                players4datacollect.Add(_client.player);
                index++;
            }
        }
    }
    public IEnumerator ShowRoundEndScreen()
    {
        Debug.Log("Ending round...");
        if (!lastRound)
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(LoadNextLevel());
            ScoreboardUI.SetActive(true);
            PlayerUI.SetActive(false);
            ClearPlayerUI();
            yield return new WaitForSeconds(4f);
            ScoreboardUI.SetActive(false);
            StartRound();
        }
        else
        {
            //screen fade effect...?
            //start endgame
            Debug.LogWarning("loaded winscreen");
            yield return new WaitForSeconds(1f);
            ScoreboardUI.SetActive(true);
            PlayerUI.SetActive(false);
            ClearPlayerUI();
            StartCoroutine(LoadLevel(winScene));
            ParseStats();
            while (isLoading)
            {
                yield return null;
            }
            ScoreboardUI.SetActive(false);
            if (StatScreen.instance == null)
            {
                print("null");
            }
            StatScreen.instance.PassStats(playerStats, match, clients);
        }
    }

    IEnumerator LoadNextLevel()
    {
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(queue[0].scene, LoadSceneMode.Single);
        isLoading = true;
        while (!asyncLoadLevel.isDone)
        {
            print("Loading the Scene");
            yield return null;
        }
        LoadLevelData();
        isLoading = false;
        queue.RemoveAt(0);
        if (queue.Count == 0)
        {
            lastRound = true;
        }
    }
    IEnumerator LoadLevel(SceneField scene, bool loadData = false)
    {
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Single);
        isLoading = true;
        while (!asyncLoadLevel.isDone)
        {
            print("Loading the Scene");
            yield return null;
        }
        if (loadData)
        {
            LoadLevelData();
        }
        isLoading = false;
    }
    #endregion
    #region Misc
    public void SetQueue(List<Level> _queue)
    {
        queue = _queue;
    }
    public void CreateClientsList()
    {
        foreach (Client client in Server.clients.Values)
        {
            if (client.connected == true && client.player == null)
            {
                clients.Add(client);
                playerStats.Add(new Stats());
            }
        }
        foreach (Client client in clients)
        {
            Debug.Log($"id: {client.id}");
        }
    }
    #endregion
    #region Debug
    public void PrintStatistic(Stats _stats)
    {
        Debug.Log($"displaying stats for: {_stats.client.username}");
        Debug.Log($"shots: {_stats.shots}");
        Debug.Log($"closeCalls: {_stats.closeCalls}");
        Debug.Log($"ADTotal: {_stats.ADTotal}");
        Debug.Log($"kills: {_stats.kills}");
        Debug.Log($"deaths: {_stats.deaths}");
        Debug.Log($"shieldBlocks: {_stats.shieldBlocks}");
        Debug.Log($"landminesCreated: {_stats.landminesCreated}");
        Debug.Log($"landmineKills: {_stats.landmineKills}");
    }
    #endregion
    #region Stats
    public void ParseStats()
    {
        foreach (RoundStats round in matchStats)
        {
            foreach (Stats stat in round.playerStats)
            {
                match.shots += stat.shots;
                match.ADTotal += stat.ADTotal;
            }
        }

        foreach (RoundStats round in matchStats)
        {
            foreach (Stats stat in round.playerStats)
            {

                playerStats[stat.client.id - 1].shots += stat.shots;
                playerStats[stat.client.id - 1].closeCalls += stat.closeCalls;
                playerStats[stat.client.id - 1].ADTotal += stat.ADTotal;
                playerStats[stat.client.id - 1].kills += stat.kills;
                playerStats[stat.client.id - 1].deaths += stat.deaths;
                playerStats[stat.client.id - 1].shieldBlocks += stat.shieldBlocks;
                playerStats[stat.client.id - 1].landminesCreated += stat.landminesCreated;
                playerStats[stat.client.id - 1].landmineKills += stat.landmineKills;

            }
        }
    }
    #endregion
}