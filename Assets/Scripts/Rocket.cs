using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed;
    public float punch;
    public GameObject HitEffect;
    public GameObject audioPlayer;
    public GameObject shieldAudio;
    public SelfDestruct smokeTrail;
    public Player sender;


    private void FixedUpdate()
    {
        transform.position += transform.forward * speed;
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogError($"hit!! {other.name}");
        Player hitPlayer;
        if (other.TryGetComponent<Player>(out hitPlayer))
        {
            if(hitPlayer == sender)
            {
                return;
            }
            else
            {
                hitPlayer.BulletDie();
                Debug.Log($"killed: {hitPlayer.username}");
                sender.kills++;
            }
        }
        if (!other.CompareTag("CloseCallBox"))
        {
            GameObject instance;
            if (other.CompareTag("Shield"))
            {
                GameObject shieldsound = Instantiate(shieldAudio);
                if (other.GetComponent<ShieldScript>().player == sender)
                {
                    return;
                }
            }
            else
            { 
                instance = Instantiate(audioPlayer);
                instance.transform.parent = GameObject.FindGameObjectWithTag("Effects").transform;
            }
            instance = Instantiate(HitEffect, transform.position, Quaternion.identity);
            smokeTrail.gameObject.transform.parent = GameObject.FindGameObjectWithTag("Effects").transform;
            smokeTrail.Destroy();
            Rigidbody rb = other.attachedRigidbody;
            if (rb)
            {
                rb.AddForce(transform.forward * punch);
            }
            Destroy(this.gameObject);
        }
    }


    private void Awake()
    {
        transform.parent = GameObject.FindGameObjectWithTag("Rockets").transform;
        StartCoroutine(Destroy());
    }


    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(20);
        Destroy(this.gameObject);
    }
}
