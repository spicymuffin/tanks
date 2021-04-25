using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutogunScript : MonoBehaviour

{
    public GameObject rocket;
    public Transform tip2;
    public float maxBulletDeviationAngle = 3.00f;
    public float fireDelay;
    public float brokenRocketDelay;
    public Coroutine fireCoroutine;
    public Coroutine brokenAuto;
    public bool isBroken = false;
    private Player nullPlayer = new Player();
    public bool decorative = false;
    public AudioSource asource;

    public void Awake()
    {
        if (!decorative)
        {
            fireCoroutine = StartCoroutine(FireDelay());
        }
    }

    public void OnTriggerEnter(Collider rocket)
    {
        if (rocket.CompareTag("Rocket"))
        {
            Debug.Log("gay");
            brokenAuto = StartCoroutine(BrokenAuto());
        }
    }

    IEnumerator FireDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireDelay);
            GameObject currentRocket = Instantiate(rocket, tip2.position, Quaternion.Euler(tip2.rotation.eulerAngles.x, tip2.rotation.eulerAngles.y + UnityEngine.Random.Range(-maxBulletDeviationAngle, maxBulletDeviationAngle), tip2.rotation.eulerAngles.z));
            currentRocket.GetComponent<Rocket>().sender = nullPlayer;
        }
    }
    IEnumerator BrokenAuto()
    {
        if (!isBroken)
        {
            isBroken = true;
            StopCoroutine(fireCoroutine);
            yield return new WaitForSeconds(brokenRocketDelay);
            Awake();
            isBroken = false;
        }
    }

    public void ShootDecorative()
    {
        GameObject currentRocket = Instantiate(rocket, tip2.position, Quaternion.Euler(tip2.rotation.eulerAngles.x, tip2.rotation.eulerAngles.y + UnityEngine.Random.Range(-maxBulletDeviationAngle, maxBulletDeviationAngle), tip2.rotation.eulerAngles.z));
        currentRocket.GetComponent<DecorativeRocket>().sender = nullPlayer;
        asource.Play();
    }
}