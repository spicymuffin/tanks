using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("General")]
    public float speed;
    public float punch;
    [Header("Effects")]
    public GameObject hitEffect;
    [Header("Sounds")]
    public GameObject hittingTheWall;
    public GameObject shieldAudio;
    [Header("Assignables")]
    public SelfDestruct smokeTrail;
    [HideInInspector]
    public Player sender;


    private void FixedUpdate()
    {
        transform.position += transform.forward * speed;
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"hit: {other.name}");
        Player hitPlayer;
        Rigidbody rb = other.attachedRigidbody;

        if (!other.CompareTag("CloseCallBox"))
        {
            GameObject instance;
            Transform parent = other.transform.parent;

            if (parent != null)
            {
                if (parent.TryGetComponent<Player>(out hitPlayer))
                {
                    if (hitPlayer == sender)
                    {
                        return;
                    }
                    else
                    {                        
                        hitPlayer.BulletDie();
                        sender.kills++;
                        Debug.LogError($"{sender.username} killed: {sender.kills}");
                    }
                }
            }

            if (other.CompareTag("Shield"))
            {
                if (other.transform.GetComponent<ShieldScript>().player == sender)
                {
                    return;
                }
                GameObject shieldsound = Instantiate(shieldAudio);
                shieldsound.transform.parent = LevelConfig.instance.effects;
            }

            else
            {
                instance = Instantiate(hittingTheWall);
                instance.transform.parent = LevelConfig.instance.effects;
            }

            instance = Instantiate(hitEffect, transform.position, Quaternion.identity);
            instance.transform.parent = LevelConfig.instance.effects;
            smokeTrail.gameObject.transform.parent = LevelConfig.instance.effects;
            smokeTrail.Destroy();
            if (rb && !other.CompareTag("Shield"))
            {
                rb.AddForce(transform.forward * punch);
            }
            Destroy(this.gameObject);
        }
    }


    private void Awake()
    {
        transform.parent = LevelConfig.instance.rockets;
    }
}
