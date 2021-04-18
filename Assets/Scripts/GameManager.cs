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
    public AudioSource audioSource;
    public AudioClip endRoundSound;
    public AudioClip endGameSound;
    List<Level> queue;
    LevelConfig LevelConfig;

    List<Client> clients = new List<Client>();
    List<Player> playersAlive = new List<Player>();
    List<Player> players4datacollect = new List<Player>();
    List<RoundStats> matchStats = new List<RoundStats>();
    List<Stats> playerStats = new List<Stats>();
    Stats match = new Stats();
    [Header("UI")]
    public GameObject ScoreboardUI;
    public ScoreBoard scoreboard;
    public GameObject PauseUI;

    [Header("Prefabs")]
    public GameObject ScoreBoardPanelPrefab;

    public Stats total = new Stats();

    bool isLoading = false;

    private bool lastRound = false;
    public SceneField winScene;

    [System.Serializable]
    public class PlayerColor
    {
        public Color color;
        public Material material;
    }
    [Header("Colors")]
    public List<PlayerColor> colors = new List<PlayerColor>();

    [Header("Assignables")]
    public Animation curtain;

    public class RoundStats
    {
        GameManager GameManager;
        //for round-summary
        public List<Stats> playerStats = new List<Stats>();
        public List<int> kills = new List<int>();

        public void SetPlayerStats(int _id, Stats _stats)
        {
            playerStats[_id - 1] = _stats;
            kills[_id - 1] = _stats.kills + _stats.landmineKills;
            PrintStatistic(_stats);
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
                    kills.Add(0);
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
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("EventSystem"));
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
    #endregion
    #region Match
    public void StartMatch()
    {
        CreateClientsList();
        scoreboard.Initialize(clients);
        PauseUI.SetActive(true);
        if (queue.Count == 1)
        {
            lastRound = true;
        }
        StartCoroutine(LoadAndStartFirstLevel(queue[0].scene));
    }
    IEnumerator LoadAndStartFirstLevel(SceneField _scene)
    {
        curtain.Play("startround");
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
    }

    //TODO: add kill rewards
    public void EndRound(Client winner)
    {
        StartCoroutine(ShowRoundEndScreen(winner));
    }
    public void LoadLevelData()
    {
        LevelConfig = GameObject.FindGameObjectWithTag("LevelConfig").GetComponent<LevelConfig>();
    }
    public void Check4EndRound()
    {
        Debug.Log($"players alive:{playersAlive.Count}");
        if (playersAlive.Count <= 1)
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
                _client.player.StartUpUI();
                _client.player.myClient = _client;
                _client.player.SetMaterial(_client.material);
                playersAlive.Add(_client.player);
                players4datacollect.Add(_client.player);
                index++;
            }
        }
    }
    public IEnumerator ShowRoundEndScreen(Client winner)
    {
        Debug.Log("Ending round...");
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        currentRoundStats.CollectPlayerStats();
        matchStats.Add(currentRoundStats);
        winner.score += 2;

        List<int> scores = new List<int>();
        for (int i = 0; i < clients.Count; i++)
        {
            clients[i].score += currentRoundStats.kills[i];
            scores.Add(clients[i].score);
        }

        scoreboard.PassScores(scores);

        if (!lastRound)
        {
            scoreboard.Sort();
            scoreboard.SetDisplay(queue.Count);
            scoreboard.SetScores();
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(3f);
            ScoreboardUI.SetActive(true);
            scoreboard.anim.Play("endround");
            yield return new WaitForSecondsRealtime(0.8f);
            StartCoroutine(LoadNextLevel());
            Time.timeScale = 1;
            yield return new WaitForSecondsRealtime(0.25f);
            scoreboard.PlayAnim();
            audioSource.PlayOneShot(endRoundSound);
            yield return new WaitForSecondsRealtime(4f);
            StartRound();
            Time.timeScale = 0;
            scoreboard.anim.Play("startround");
            yield return new WaitForSecondsRealtime(0.4f);
            ScoreboardUI.SetActive(false);
            scoreboard.ResetFills();
            yield return new WaitForSecondsRealtime(3f);
            Time.timeScale = 1;
        }
        else
        {
            //screen fade effect...?
            //start endgame
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(3f);
            ScoreboardUI.SetActive(true);
            scoreboard.anim.Play("endround");
            yield return new WaitForSecondsRealtime(0.4f);
            StartCoroutine(LoadLevel(winScene));
            ParseStats();
            while (isLoading)
            {
                yield return null;
            }
            Time.timeScale = 1;
            ScoreboardUI.SetActive(false);
            if (StatScreen.instance == null)
            {
                print("null");
            }
            StatScreen.instance.PassStats(playerStats, match, clients);
            audioSource.PlayOneShot(endGameSound);

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

    public void LoadTotalStats()
    {
        total.shots = PlayerPrefs.GetInt("totalShots");
    }

    public void SetTotalStats()
    {
        PlayerPrefs.SetInt("totalShots", total.shots);
    }

    #endregion
}