using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using TankStatistics;
using Random = UnityEngine.Random;

//Если вдруг меня не станет бейби
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
    public float speedBoost;
    public float saveMaxVelocity;
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
    public const int MAX_ROCKETS = 7;
    public float reloadTime = 0.6f;
    public float headRotationSpeed = 0.15f;
    public float coolDownTime = 0.5f;
    private float headTargetAngle = 0;
    List<Image> rockets = new List<Image>();
    int currentRockets = MAX_ROCKETS;
    bool isReloading = false;
    bool isCoolingDown = false;
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
            player.StartCoroutine(Countdown());
        }

        IEnumerator Countdown()
        {
            player.ADrunning = true;
            player.infBullets = true;
            float t = 0.0f;
            float start = 1;
            float end = 0;
            while (t < time)
            {
                t += Time.deltaTime;
                player.ADprgbar.fillAmount = Mathf.Lerp(start, end, t / time);
                yield return null;
            }
            player.PlayTakeAnimation();
            player.infBullets = false;
            player.hasItem = false;
            player.currentItem = null;
            Destroy(player.currentItemIcon);
            player.ADrunning = false;
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
            player.StartCoroutine(Countdown());
        }

        IEnumerator Countdown()
        {
            player.ADrunning = true;
            player.isShielded = true;
            GameObject shield = Instantiate(player.shieldV1, player.shieldPos.position, Quaternion.identity);
            ShieldScript ss = shield.GetComponent<ShieldScript>();
            ss.player = player;
            ss.Appear();
            yield return new WaitForSeconds(ss.ADuration);
            shield.GetComponent<SphereCollider>().enabled = true;
            float t = 0.0f;
            float start = 1;
            float end = 0;
            while (t < time)
            {
                t += Time.deltaTime;
                player.ADprgbar.fillAmount = Mathf.Lerp(start, end, t / time);
                yield return null;
            }
            shield.GetComponent<SphereCollider>().enabled = false;
            ss.Disappear();
            yield return new WaitForSeconds(ss.ADuration);
            Destroy(shield);
            player.PlayTakeAnimation();
            player.isShielded = false;
            player.hasItem = false;
            player.currentItem = null;
            Destroy(player.currentItemIcon);
            player.ADrunning = false;
        }
    }

    public class Landmine : Item
    {
        public LandmineADC adc;
        public int count;
        public float dec;
        public Landmine(int _count, Player _player)
        {
            player = _player;
            count = _count;
            player.currentItem = this;
            player.hasItem = true;
            dec = 1 / (float)count;
        }

        public override void Use()
        {
            Destroy(player.currentItemIcon);
            if (count != 1)
            {
                player.currentItemIcon = Instantiate(adc.icon[count - 2], player.ADslot.transform);
            }
            else
            {
                player.currentItemIcon = Instantiate(adc.icon[0], player.ADslot.transform);
            }

            GameObject landmine = Instantiate(player.landmine, player.transform.position, Quaternion.identity);
            landmine.GetComponent<LandmineScript>().creator = player;
            count--;
            player.ADprgbar.fillAmount = dec * count;
            if (count == 0)
            {
                player.PlayTakeAnimation();
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
            player.StartCoroutine(Countdown());
        }

        IEnumerator Countdown()
        {
            player.ADrunning = true;
            player.isRicochet = true;
            float t = 0.0f;
            float start = 1;
            float end = 0;
            while (t < time)
            {
                t += Time.deltaTime;
                player.ADprgbar.fillAmount = Mathf.Lerp(start, end, t / time);
                yield return null;
            }
            player.PlayTakeAnimation();
            player.isRicochet = false;
            player.hasItem = false;
            player.currentItem = null;
            Destroy(player.currentItemIcon);
            player.ADrunning = false;
        }
    }

    public bool isRicochet = false;
    public bool infBullets = false;
    public bool isShielded = false;
    public bool ADrunning = false;

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
    public int totalShots = 0;
    public int totalKills = 0;
    public int totalDeaths = 0;
    #endregion
    #region Display
    [Header("Display")]
    public Image staminabar;
    public Image rocketIcon;
    public Transform rocketTray;
    public Transform ADslot;
    public TextMeshProUGUI nameDisplay;
    public Image ADprgbar;
    public Animation ADanimation;
    GameObject currentItemIcon;
    #endregion
    #region State
    private bool dead = false;
    #endregion
    #region Objects & Transforms & Rigidbodies
    private Rigidbody rb;
    [Header("Objects & Transforms")]
    public GameObject Rocket;
    public GameObject RicRocket;
    public GameObject shieldV1;
    public GameObject landmine;
    public Transform tip;
    public Transform shieldPos;
    public GameObject explosionEffect;
    public Transform head;
    public Transform canvas;
    public MeshRenderer headMR;
    public MeshRenderer hullMR;
    #endregion
    #region Audio
    [Header("Audio")]
    public GameObject explosionSound;
    public AudioSource playerSound;
    public AudioSource shotSound;
    public AudioClip shotSoundClip;
    public AudioClip pickUpAirDropSound;
    public AudioClip usingAirDropSound;
    public AudioClip pickUpAirDropCancelSound;
    #endregion
    #region Animations
    List<string> giveADs = new List<string>() { "giveAD0", "giveAD1", "giveAD2", "giveAD3", "giveAD4" };
    List<string> takeADs = new List<string>() { "takeAD0", "takeAD1", "takeAD2", "takeAD3", "takeAD4" };
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
        canvas.SetParent(null);
    }
    #endregion
    #region Fire Functions
    Coroutine reloadCoroutine;
    Coroutine coolDownCoroutine;
    /// <summary>
    /// Reload with delay
    /// </summary>
    /// <returns></returns>
    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        AddBullet();
        isReloading = false;
    }
    /// <summary>
    /// Cool down
    /// </summary>
    /// <returns></returns>
    IEnumerator FireCoolDown()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(coolDownTime);
        isCoolingDown = false;
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
    /// <summary>
    /// Fires a rocket
    /// </summary>
    private void Fire()
    {
        if (currentRockets != 0)
        {
            if (isReloading && !infBullets)
            {
                StopCoroutine(reloadCoroutine);
                reloadCoroutine = StartCoroutine(Reload());
            }
            shotSound.pitch = Random.Range(1.0f, 1.15f);
            shotSound.PlayOneShot(shotSoundClip);
            shots++;
            rb.velocity /= 6;
            if (!isRicochet)
            {
                GameObject currentRocket = Instantiate(Rocket, tip.position, Quaternion.Euler(tip.rotation.eulerAngles.x, tip.rotation.eulerAngles.y + UnityEngine.Random.Range(-maxBulletDeviationAngle, maxBulletDeviationAngle), tip.rotation.eulerAngles.z));
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
            //Debug.Log($"-bullet: {currentRockets}");
        }
    }
    /// <summary>
    /// Manage head rotation and firing
    /// </summary>
    private void HeadRotationAndFire()
    {
        Vector2 jsVector = new Vector2(input.FH, input.FV);
        float magnitude = jsVector.magnitude;

        if (input.FH != 0 || input.FV != 0)
        {
            headTargetAngle = Mathf.Atan2(input.FH, input.FV) * Mathf.Rad2Deg;
            head.rotation = Quaternion.Lerp(head.rotation, Quaternion.Euler(0, headTargetAngle + 180, 0), headRotationSpeed);
            if (!isCoolingDown)
            {
                Fire();
                coolDownCoroutine = StartCoroutine(FireCoolDown());
            }
        }
        else
        {
            if (coolDownCoroutine != null)
            {
                StopCoroutine(coolDownCoroutine);
                coolDownCoroutine = null;
                isCoolingDown = false;
            }
        }
        if (currentRockets < MAX_ROCKETS && !isReloading)
        {
            //Debug.Log($"start reload");
            reloadCoroutine = StartCoroutine(Reload());
        }
    }
    #endregion
    #region UI Functions
    /// <summary>
    /// Start up UI
    /// </summary>
    /// <param name="_panelPosGameObject"></param>
    public void StartUpUI()
    {
        nameDisplay.text = username;
        //Start bulllets
        for (int i = 0; i < MAX_ROCKETS; i++)
        {
            rockets.Add(Instantiate(rocketIcon, rocketTray.transform));
        }
    }
    /// <summary>
    /// Update UI
    /// </summary>
    private void UpdateUI()
    {
        if (staminabar != null)
        {
            staminabar.fillAmount = sprintStamina;
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
        if (use && hasItem && !ADrunning)
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
    #region Unity calls
    public void Start()
    {
        shotSound = GetComponent<AudioSource>();
    }
    private void Awake()
    {
        StartUp();
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
        canvas.position = transform.position + new Vector3(0, 1.9f, 0);
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("BoostPad"))
        {
            maxVelocity += speedBoost;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BoostPad"))
        {
            maxVelocity = 6f;
        }
    }
    #endregion
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
            if (rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity *= 0.97f;
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
        GameObject instance;
        instance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        instance.transform.parent = LevelConfig.instance.effects;
        instance = Instantiate(explosionSound, transform.position, Quaternion.identity);
        instance.transform.parent = LevelConfig.instance.effects;
        rb.velocity = Vector3.zero;
        print(kills);
        GameManager.instance.KillPlayer(this);
    }

    public void ExplosionDie()
    {
        deaths++;
        dead = true;
        CameraController.instance.Shake();
        rb.velocity = Vector3.zero;
        GameManager.instance.KillPlayer(this);
    }

    public void Die()
    {
        StopAllCoroutines();
        deaths++;
        dead = true;
        rb.velocity = Vector3.zero;
        GameManager.instance.KillPlayer(this);
    }
    #endregion
    #region Misc Functions

    public void PlayTakeAnimation()
    {
        int rnd = UnityEngine.Random.Range(1, 101);
        string anim = "";

        if (rnd == 100)
        {
            anim = takeADs[0];
        }
        else if (rnd <= 60)
        {
            anim = takeADs[4];
        }
        else if (rnd <= 80)
        {
            anim = takeADs[1];
        }
        else if (rnd <= 95)
        {
            anim = takeADs[3];
        }
        else
        {
            anim = takeADs[2];
        }

        ADanimation.Play(anim);
    }

    /// <summary>
    /// Give an airdrop to the player
    /// </summary>
    /// <param name="_type">Type of the airdrop (is located in the adc script)</param>
    /// <param name="_airdrop">The airdrop gameobject</param>
    public void GiveAirdrop(string _type, GameObject _airdrop)
    {
        ADprgbar.fillAmount = 1f;
        int rnd = UnityEngine.Random.Range(1, 101);
        string anim = "";
        print(rnd);

        if (rnd == 100)
        {
            anim = giveADs[0];
        }
        else if (rnd <= 60)
        {
            anim = giveADs[4];
        }
        else if (rnd <= 80)
        {
            anim = giveADs[1];
        }
        else if (rnd <= 95)
        {
            anim = giveADs[3];
        }
        else
        {
            anim = giveADs[2];
        }

        ADTotal++;
        if (hasItem || currentItem != null)
        {
            //play animation or some shit idk
            playerSound.PlayOneShot(pickUpAirDropCancelSound);
            return;
        }

        if (_type == "InfiniteBullets")
        {
            playerSound.PlayOneShot(pickUpAirDropSound);
            InfiniteBulletsADC adc = _airdrop.GetComponent<InfiniteBulletsADC>();
            currentItemIcon = Instantiate(adc.icon, ADslot.transform);
            new InfiniteBullets(adc.time, this);
        }
        if (_type == "LandMine")
        {
            playerSound.PlayOneShot(pickUpAirDropSound);
            LandmineADC adc = _airdrop.GetComponent<LandmineADC>();
            currentItemIcon = Instantiate(adc.icon[adc.count - 1], ADslot.transform);
            new Landmine(adc.count, this);
            var item = currentItem as Landmine;
            item.adc = adc;
        }
        if (_type == "Shield")
        {
            playerSound.PlayOneShot(pickUpAirDropSound);
            ShieldADC adc = _airdrop.GetComponent<ShieldADC>();
            currentItemIcon = Instantiate(adc.icon, ADslot.transform);
            new Shield(adc.time, this);
        }
        if (_type == "Ricochet")
        {
            playerSound.PlayOneShot(pickUpAirDropSound);
            RicochetADC adc = _airdrop.GetComponent<RicochetADC>();
            currentItemIcon = Instantiate(adc.icon, ADslot.transform);
            new Ricochet(adc.time, this);
        }
        ADanimation.Play(anim);
    }

    /// <summary>
    /// Pack statistics and send them to the GameManager
    /// </summary>
    public void SendStats()
    {
        Stats myStats = new Stats(shots, closeCalls, ADTotal, kills, deaths, shieldBlocks, landminesCreated, landmineKills, myClient);
        GameManager.instance.currentRoundStats.SetPlayerStats(id, myStats);
        print(kills);
        SendTotalStats();
    }


    public void SendTotalStats()
    {
        PlayerPrefs.SetInt("totalShots", PlayerPrefs.GetInt("totalShots") + shots);
        PlayerPrefs.SetInt("totalKills", PlayerPrefs.GetInt("totalKills") + kills);
        PlayerPrefs.SetInt("totalDeaths", PlayerPrefs.GetInt("totalDeaths") + deaths);
        //Debug.LogError("asd");
    }
    /// <summary>
    /// Change player color
    /// </summary>
    /// <param name="material"> material to set</param>
    public void SetMaterial(Material material)
    {
        headMR.material = material;
        hullMR.material = material;
    }
    #endregion
}
