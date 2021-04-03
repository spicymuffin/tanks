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
    #region Networking
    [Header("Networking")]
    public int id;
    public string username;
    public Client myClient;
    public struct Input
    {
        public bool sprint;
        public bool use;
        public float MH;
        public float MV;
        public float FH;
        public float FV;
    }
    public Input input;
    #endregion
    #region Movement
    private float hullTargetAngle = 0;
    [Header("Movement")]
    public float hullRotationSpeed = 0.15f;
    public float maxVelocity = 6f;
    public float acceleration = 50;
    #endregion
    #region Counter movement
    [Header("Counter movement")]
    public float counterMovement = 0.6f;
    #endregion
    #region Input processing
    private bool sprint = false;
    private bool use = false;

    private bool lastInputSprint = false;
    private bool lastInputUse = false;
    #endregion
    #region Sprint
    [Header("Sprint")]
    public float sprintPropulsion = 12f;
    public float sprintStamina = 1;
    public float sprintStaminaRegenRate = 0.2f;
    public float sprintStaminaDepletionRate = 0.4f;
    public bool isDepleting = false;
    #endregion
    #region Fire
    [Header("Fire")]
    public float maxBulletDeviationAngle = 1.00f;
    public const int MAX_ROCKETS = 5;
    public float reloadTime = 0.6f;
    public float headRotationSpeed = 0.15f;
    private float headTargetAngle = 0;
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
    [Header("Stats")]
    public int shots = 0;
    public int closeCalls = 0;
    public int ADTotal = 0;
    public int kills = 0;
    public int deaths = 0;
    public int shieldBlocks = 0;
    public int landminesCreated = 0;
    public int landmineKills = 0;
    #endregion
    #region NameDisplay
    [Header("Name display")]
    public TextMeshProUGUI nameDisplay;
    #endregion
    #region State
    private bool dead = false;
    #endregion
    #region Objects & Transforms & Rigidbodies
    private Rigidbody rb;
    [Header("Objects & Transforms")]
    public GameObject Rocket;
    public GameObject RicRocket;
    public GameObject shield;
    public GameObject landmine;
    public Transform tip;
    public Transform shieldPos;
    public Image rocketIcon;
    public GameObject UIPanel;
    public GameObject explosionEffect;
    public Transform head;
    #endregion
    #region Audio
    [Header("Audio")]
    public GameObject explosionSound;
    public AudioSource playerSound;
    public AudioSource shotSound;
    public AudioClip pickUpAirDropSound;
    public AudioClip usingAirDropSound;
    public AudioClip pickUpAirDropCancelSound;
    #endregion

    #region Startup Functions
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
    /// <summary>
    /// Startup this script
    /// </summary>
    private void StartUp()
    {
        rb = GetComponent<Rigidbody>();
    }
    #endregion
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
        //Debug.Log($"+bullet: {currentRockets}");
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
        nameDisplay.text = username;
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
        Sprint();
        Use();
    }
    /// <summary>
    /// Preprocess input
    /// </summary>
    private void ProcessInput()
    {
        if (!lastInputSprint && input.sprint) { sprint = true; }
        if (lastInputSprint && input.sprint) { sprint = false; }
        if (!input.sprint) { sprint = false; }
        if (!lastInputUse && input.use) { use = true; }
        if (lastInputUse && input.use) { use = false; }
        if (!input.use) { use = false; }
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
    }
    /// <summary>
    /// Uses Item
    /// </summary>
    private void Use()
    {
        if (use && hasItem)
        {
            playerSound.PlayOneShot(usingAirDropSound);
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
        Debug.Log($"sprint: {_input.sprint}");
        Debug.Log($"use: {_input.use}");
        Debug.Log($"Mhorizontal: {_input.MH}");
        Debug.Log($"Mvertical: {_input.MV}");
        Debug.Log($"Fhorizontal: {_input.FH}");
        Debug.Log($"Fvertical: {_input.FV}");
    }
    #endregion
    public void Start()
    {
        shotSound = GetComponent<AudioSource>();
    }


    /// <summary>
    /// Fires a rocket
    /// </summary>
    private void Fire()
    {
        //if (fire && currentRockets != 0)
        {
            if (isReloading)
            {
                //Debug.Log("restart reload");
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
            //Debug.Log($"-bullet: {currentRockets}");
        }
        //if (currentRockets < MAX_ROCKETS && !isReloading && !fire)
        {
            //Debug.Log($"start reload");
            reloadCoroutine = StartCoroutine(Reload());
        }
    }

    #region Movement Functions
    /// <summary>
    /// Main movement loop
    /// </summary>
    private void Movement()
    {
        Vector2 jsVector = new Vector2(input.MH, input.MV);
        float magnitude = jsVector.magnitude;
        CounterMovement(magnitude);

        if (input.MH != 0 || input.MV != 0)
        {
            hullTargetAngle = Mathf.Atan2(input.MH, input.MV) * Mathf.Rad2Deg;
            Quaternion prevRot = head.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, hullTargetAngle, 0), hullRotationSpeed);
            head.rotation = prevRot;

            if (rb.velocity.magnitude < maxVelocity * magnitude)
            {
                rb.AddForce(transform.forward * acceleration);
            }
        }
    }

    /// <summary>
    /// Apply counter movement
    /// </summary>
    private void CounterMovement(float magnitude)
    {
        Vector2 mag = new Vector2(transform.InverseTransformDirection(rb.velocity).x, transform.InverseTransformDirection(rb.velocity).z);

        if (Math.Abs(mag.x) > 0.01)
        {
            rb.AddForce(acceleration * transform.right * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > 0.01 && Math.Abs(magnitude) < 0.05f || (mag.y < -0.01 && magnitude > 0) || (mag.y > 0.01 && magnitude < 0))
        {
            rb.AddForce(acceleration * transform.forward * -mag.y * counterMovement);
        }
    }

    private void HeadRotationAndFire()
    {
        Vector2 jsVector = new Vector2(input.FH, input.FV);
        float magnitude = jsVector.magnitude;

        if (input.FH != 0 || input.FV != 0)
        {
            headTargetAngle = Mathf.Atan2(input.FH, input.FV) * Mathf.Rad2Deg;
            head.rotation = Quaternion.Lerp(head.rotation, Quaternion.Euler(0, headTargetAngle + 180, 0), headRotationSpeed);
            if (!isReloading)
            {
                Fire();
            }
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
        dead = true;
        CameraController.instance.Shake();
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
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
        CameraController.instance.Shake();
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        GameManager.instance.KillPlayer(this);
    }

    public void ExplosionDie()
    {
        deaths++;
        dead = true;
        CameraController.instance.Shake();
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
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
            playerSound.PlayOneShot(pickUpAirDropCancelSound);
            return;
        }

        if (_type == "InfiniteBullets")
        {
            playerSound.PlayOneShot(pickUpAirDropSound);
            // pickUpAirDropSound.Play();
            InfiniteBulletsADC adc = _airdrop.GetComponent<InfiniteBulletsADC>();
            currentItemIcon = Instantiate(adc.icon, myUIItemPanel.transform);
            new InfiniteBullets(adc.time, this);
        }
        if (_type == "LandMine")
        {
            playerSound.PlayOneShot(pickUpAirDropSound);
            // pickUpAirDropSound.Play();
            LandmineADC adc = _airdrop.GetComponent<LandmineADC>();
            currentItemIcon = Instantiate(adc.icon[adc.count - 1], myUIItemPanel.transform);
            new Landmine(adc.count, this);
            var item = currentItem as Landmine;
            item.adc = adc;
        }
        if (_type == "Shield")
        {
            playerSound.PlayOneShot(pickUpAirDropSound);
            // pickUpAirDropSound.Play();
            ShieldADC adc = _airdrop.GetComponent<ShieldADC>();
            currentItemIcon = Instantiate(adc.icon, myUIItemPanel.transform);
            new Shield(adc.time, this);
        }
        if (_type == "Ricochet")
        {
            playerSound.PlayOneShot(pickUpAirDropSound);
            // pickUpAirDropSound.Play();
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
        //LogInput(input);
        ProcessInput();
        if (!dead)
        {
            Movement();
            HeadRotationAndFire();
            ButtonInput();
            UpdateUI();
            lastInputSprint = input.sprint;
            lastInputUse = input.use;
        }
        nameDisplay.gameObject.transform.parent.rotation = Quaternion.Euler(90, 0, 0);
    }

    private void Awake()
    {
        StartUp();
    }
}
