using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using TankStatistics;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{

    #region Audio
    public AudioSource shotSound;
    public AudioSource pickUpAirDropSound;
    public AudioSource usingAirDropSound;
    #endregion

    #region Networking
    public int id;
    public string username;
    public Client myClient;
    public struct Input
    {
        public bool fire;
        public bool sprint;
        public bool use;
        public float horizontal;
        public float vertical;
    }
    public Input input;
    #endregion

    #region Assignables
    public float rotSens = 200f;
    public float maxVelocity = 12f;
    public float acceleration = 100000;
    public float sprintPropulsion = 12f;
    #endregion

    #region Objects & Transforms & Rigidbodies
    private Rigidbody rb;
    public GameObject Rocket;
    public GameObject RicRocket;
    public GameObject shield;
    public GameObject landmine;
    public Transform tip;
    public Transform shieldPos;
    public TextMeshPro nameDisplay;
    public Image staminaBar;
    public Image rocketIcon;
    public GameObject UIPanel;
    public BoxCollider closeCallBox;
    #endregion

    #region Counter movement
    public float counterMovement = 0.6f;
    public float threshold = 0.01f;
    public float jsThreshold = 0.05f;
    #endregion

    #region Input processing
    private bool fire = false;
    private bool sprint = false;
    private bool use = false;

    private bool lastInputFire = false;
    private bool lastInputSprint = false;
    private bool lastInputUse = false;
    #endregion

    #region Sprint
    public float sprintStamina = 1;
    public float sprintStaminaRegenRate = 0.2f;
    public float sprintStaminaDepletionRate = 0.4f;
    public bool isDepleting = false;
    #endregion

    #region Fire
    public float maxBulletDeviationAngle = 3.00f;
    public const int MAX_ROCKETS = 5;
    public float reloadTime = 0.6f;
    List<Image> rockets = new List<Image>();
    int currentRockets = MAX_ROCKETS;
    bool isReloading = false;
    #endregion

    #region UIPanel
    GameObject myUIPanel;
    TextMeshProUGUI myUINameDisplay;
    Image myUIStaminaBar;
    GameObject myUIRocketPanel;
    GameObject myUIItemPanel;
    GameObject currentItemIcon;
    #endregion

    #region Airdrop logic
    bool hasItem = false;
    Item currentItem = null;

    public abstract class Item
    {
        public Player player;
        public abstract void Use();
    }

    public class InfiniteBullets : Item
    {
        public float time;

        public InfiniteBullets(float _time, Player _player)
        {
            player = _player;
            time = _time;
            player.currentItem = this;
            player.hasItem = true;
        }

        public override void Use()
        {
            Destroy(player.currentItemIcon);
            player.StartCoroutine(Countdown());
            player.hasItem = false;
            player.currentItem = null;
        }

        IEnumerator Countdown()
        {
            player.infBullets = true;
            yield return new WaitForSeconds(time);
            player.infBullets = false;
        }
    }

    public class Shield : Item
    {
        public float time;
        public Shield(float _time, Player _player)
        {
            player = _player;
            time = _time;
            player.currentItem = this;
            player.hasItem = true;
        }

        public override void Use()
        {
            Destroy(player.currentItemIcon);
            player.StartCoroutine(Countdown());
            player.hasItem = false;
            player.currentItem = null;
        }

        IEnumerator Countdown()
        {
            player.isShielded = true;
            GameObject shield = Instantiate(player.shield, player.shieldPos);
            shield.GetComponent<ShieldScript>().player = player;
            yield return new WaitForSeconds(time);
            Destroy(shield);
            player.isShielded = false;
        }
    }

    public class Landmine : Item
    {
        public LandmineADC adc;
        public int count;
        public Landmine(int _count, Player _player)
        {
            player = _player;
            count = _count;
            player.currentItem = this;
            player.hasItem = true;
        }

        public override void Use()
        {
            Destroy(player.currentItemIcon);
            if (count != 1)
            {
                player.currentItemIcon = Instantiate(adc.icon[count - 2], player.myUIItemPanel.transform);
            }
            else
            {
                player.currentItemIcon = Instantiate(adc.icon[0], player.myUIItemPanel.transform);
            }

            GameObject landmine = Instantiate(player.landmine, player.transform.position, Quaternion.identity);
            landmine.GetComponent<LandmineScript>().sender = player;
            count--;
            if (count == 0)
            {
                player.hasItem = false;
                player.currentItem = null;
                Destroy(player.currentItemIcon);
            }
        }
    }

    public class Ricochet : Item
    {
        public float time;

        public Ricochet(float _time, Player _player)
        {
            player = _player;
            time = _time;
            player.currentItem = this;
            player.hasItem = true;
        }

        public override void Use()
        {
            Destroy(player.currentItemIcon);
            player.StartCoroutine(Countdown());
            player.hasItem = false;
            player.currentItem = null;
        }

        IEnumerator Countdown()
        {
            player.isRicochet = true;
            yield return new WaitForSeconds(time);
            player.isRicochet = false;
        }
    }

    bool isRicochet = false;
    bool infBullets = false;
    bool isShielded = false;

    #endregion

    #region Stats
    public int shots;
    public int closeCalls;
    public int ADTotal;
    public int kills;
    public int deaths;
    public int shieldBlocks;
    public int landminesCreated;
    public int landmineKills;
    #endregion

    #region State
    bool dead = false; 
    #endregion

    /// <summary>
    /// Initialize to NM
    /// </summary>
    /// <param name="_id">ID of player</param>
    /// <param name="_username">Name of player</param>
    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
    }

    public void Start()
    {
        shotSound = GetComponent<AudioSource>();
    }
    /// <summary>
    /// Startup this script
    /// </summary>
    private void StartUp()
    {
        rb = GetComponent<Rigidbody>();
        nameDisplay.text = username;
    }

    #region Reload Functions
    Coroutine reloadCoroutine;
    /// <summary>
    /// Reload with delay
    /// </summary>
    /// <returns></returns>
    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        AddBullet();
        Debug.Log($"+bullet: {currentRockets}");
        isReloading = false;
    }
    /// <summary>
    /// Add a bullet + update UI
    /// </summary>
    private void AddBullet()
    {
        currentRockets++;
        rockets[currentRockets - 1].enabled = true;
    }
    /// <summary>
    /// Remove a bullet + update UI
    /// </summary>
    private void RemoveBullet()
    {
        rockets[currentRockets - 1].enabled = false;
        currentRockets--;
    } 
    #endregion

    #region UI Functions
    /// <summary>
    /// Start up UI
    /// </summary>
    /// <param name="_panelPosGameObject"></param>
    public void StartUpUI(GameObject _panelPosGameObject)
    {
        myUIPanel = Instantiate(UIPanel, _panelPosGameObject.transform);
        myUINameDisplay = myUIPanel.transform.FindObjectsWithTag("UIName")[0].GetComponent<TextMeshProUGUI>();
        myUIStaminaBar = myUIPanel.transform.FindObjectsWithTag("UIStamina")[0].GetComponent<Image>();
        myUIRocketPanel = myUIPanel.transform.FindObjectsWithTag("UIBullet")[0];
        myUIItemPanel = myUIPanel.transform.FindObjectsWithTag("UIItem")[0];
        myUINameDisplay.text = username;
        //Start bulllets
        for (int i = 0; i < MAX_ROCKETS; i++)
        {
            rockets.Add(Instantiate(rocketIcon, myUIRocketPanel.transform));
        }
    }
    /// <summary>
    /// Update UI
    /// </summary>
    private void UpdateUI()
    {
        if (myUIStaminaBar != null)
        {
            myUIStaminaBar.fillAmount = sprintStamina;
        }
    } 
    #endregion

    #region Input handling Functions
    /// <summary>
    /// Check input, activate functions accordingly
    /// </summary>
    private void ButtonInput()
    {
        Fire();
        Sprint();
        Use();
    }
    /// <summary>
    /// Preprocess input
    /// </summary>
    private void ProcessInput()
    {
        if (!lastInputFire && input.fire) { fire = true; }
        if (lastInputFire && input.fire) { fire = false; }
        if (!input.fire) { fire = false; }
        if (!lastInputSprint && input.sprint) { sprint = true; }
        if (lastInputSprint && input.sprint) { sprint = false; }
        if (!input.sprint) { sprint = false; }
        if (!lastInputUse && input.use) { use = true; }
        if (lastInputUse && input.use) { use = false; }
        if (!input.use) { use = false; }
    }
    /// <summary>
    /// Fires a rocket
    /// </summary>
    private void Fire()
    {
        if (fire && currentRockets != 0)
        {
            if (isReloading)
            {
                Debug.Log("restart reload");
                StopCoroutine(reloadCoroutine);
                reloadCoroutine = StartCoroutine(Reload());
            }
            if (!isRicochet)
            {
                GameObject currentRocket = Instantiate(Rocket, tip.position, Quaternion.Euler(tip.rotation.eulerAngles.x, tip.rotation.eulerAngles.y + UnityEngine.Random.Range(-maxBulletDeviationAngle, maxBulletDeviationAngle), tip.rotation.eulerAngles.z));
                shotSound.pitch = Random.Range(1, 2);
                shotSound.Play();
                currentRocket.GetComponent<Rocket>().sender = this;
            }
            else
            {
                GameObject currentRicRocket = Instantiate(RicRocket, tip.position, Quaternion.Euler(tip.rotation.eulerAngles.x, tip.rotation.eulerAngles.y + UnityEngine.Random.Range(-maxBulletDeviationAngle, maxBulletDeviationAngle), tip.rotation.eulerAngles.z));
                currentRicRocket.GetComponent<RicRocket>().sender = this;
            }
            if (!infBullets)
            {
                RemoveBullet();
            }
            shots++;
            Debug.Log($"-bullet: {currentRockets}");
        }
        if (currentRockets < MAX_ROCKETS && !isReloading && !fire)
        {
            Debug.Log($"start reload");
            reloadCoroutine = StartCoroutine(Reload());
        }
    }
    /// <summary>
    /// Activates sprint
    /// </summary>
    private void Sprint()
    {
        if (sprint && sprintStamina > 0.96f)
        {
            isDepleting = true;
            rb.AddForce(transform.forward * sprintPropulsion, ForceMode.VelocityChange);
        }
        else if (!sprint && sprintStamina < 1 && !isDepleting)
        {
            sprintStamina += sprintStaminaRegenRate;
        }
        else if (isDepleting)
        {
            sprintStamina -= sprintStaminaDepletionRate;
            if (sprintStamina < 0.09f)
            {
                isDepleting = false;
            }
        }
        staminaBar.fillAmount = sprintStamina;
    }
    /// <summary>
    /// Uses Item
    /// </summary>
    private void Use()
    {
        if (use && hasItem)
        {
            usingAirDropSound.Play();
            currentItem.Use();
        }
    }
    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_input">The new key input.</param>
    public void SetInput(Input _input)
    {
        input = _input;
    }
    /// <summary>
    /// Log input
    /// </summary>
    /// <param name="_input"></param>
    public void LogInput(Input _input)
    {
        Debug.Log($"fire: {_input.fire}");
        Debug.Log($"sprint: {_input.sprint}");
        Debug.Log($"use: {_input.use}");
        Debug.Log($"horizontal: {_input.horizontal}");
        Debug.Log($"vertical: {_input.vertical}");
    }
    #endregion

    #region Movement Functions
    /// <summary>
    /// Main movement loop
    /// </summary>
    private void Movement()
    {
        CounterMovement();
        transform.Rotate(0, input.horizontal * rotSens, 0);

        if (rb.velocity.magnitude < maxVelocity)
        {
            rb.AddForce(transform.forward * acceleration * input.vertical);
        }
    }
    /// <summary>
    /// Apply counter movement
    /// </summary>
    private void CounterMovement()
    {
        Vector2 mag = new Vector2(transform.InverseTransformDirection(rb.velocity).x, transform.InverseTransformDirection(rb.velocity).z);
        float x = input.horizontal;
        float y = input.vertical;
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < jsThreshold || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0) || (mag.x < -threshold && x < 0) || (mag.x > threshold && x > 0))
        {
            rb.AddForce(acceleration * transform.right * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < jsThreshold || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(acceleration * transform.forward * -mag.y * counterMovement);
        }
    } 
    #endregion

    #region Die Functions
    /// <summary>
    /// Die from a bullet
    /// </summary>
    public void BulletDie()
    {
        StopAllCoroutines();
        closeCalls--;
        deaths++;
        if (isShielded)
        {
            shieldBlocks--;
        }
        dead = true;
        GameManager.instance.KillPlayer(this);
    }
    /// <summary>
    /// Die from a landmine
    /// </summary>
    public void LandmineDie()
    {
        StopAllCoroutines();
        deaths++;
        dead = true;
        GameManager.instance.KillPlayer(this);
    }

    public void Die()
    {
        StopAllCoroutines();
        deaths++;
        dead = true;
        GameManager.instance.KillPlayer(this);
    }
    #endregion

    /// <summary>
    /// Give an airdrop to the player
    /// </summary>
    /// <param name="_type">Type of the airdrop (is located in the adc script)</param>
    /// <param name="_airdrop">The airdrop gameobject</param>
    public void GiveAirdrop(string _type, GameObject _airdrop)
    {
        ADTotal++;
        if (hasItem)
        {
            //play animation or some shit idk
            return;
        }

        if (_type == "InfiniteBullets")
        {
            pickUpAirDropSound.Play();
            InfiniteBulletsADC adc = _airdrop.GetComponent<InfiniteBulletsADC>();
            currentItemIcon = Instantiate(adc.icon, myUIItemPanel.transform);
            new InfiniteBullets(adc.time, this);
        }
        if (_type == "LandMine")
        {
            pickUpAirDropSound.Play();
            LandmineADC adc = _airdrop.GetComponent<LandmineADC>();
            currentItemIcon = Instantiate(adc.icon[adc.count - 1], myUIItemPanel.transform);
            new Landmine(adc.count, this);
            var item = currentItem as Landmine;
            item.adc = adc;
        }
        if (_type == "Shield")
        {
            pickUpAirDropSound.Play();
            ShieldADC adc = _airdrop.GetComponent<ShieldADC>();
            currentItemIcon = Instantiate(adc.icon, myUIItemPanel.transform);
            new Shield(adc.time, this);
        }
        if (_type == "Ricochet")
        {
            pickUpAirDropSound.Play();
            RicochetADC adc = _airdrop.GetComponent<RicochetADC>();
            currentItemIcon = Instantiate(adc.icon, myUIItemPanel.transform);
            new Ricochet(adc.time, this);
        }
    }

    /// <summary>
    /// Pack statistics and send them to the GameManager
    /// </summary>
    public void SendStats()
    {
        Stats myStats = new Stats(shots, closeCalls, ADTotal, kills, deaths, shieldBlocks, landminesCreated, landmineKills, myClient);
        GameManager.instance.currentRoundStats.SetPlayerStats(id, myStats);
    }
    private void FixedUpdate()
    {
        ProcessInput();
        if (!dead)
        {
            Movement();
            ButtonInput();
            UpdateUI();
            lastInputFire = input.fire;
            lastInputSprint = input.sprint;
            lastInputUse = input.use;
            nameDisplay.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
    private void Awake()
    {
        StartUp();
    }
}
