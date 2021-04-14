using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed;
    public float punch;
    public GameObject HitEffect;
    public GameObject hittingTheWall;
    public GameObject shieldAudio;
    public SelfDestruct smokeTrail;
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
                        Debug.Log($"killed: {hitPlayer.username}");
                        hitPlayer.BulletDie();
                        sender.kills++;
                    }
                }
            }

            if (other.CompareTag("Shield"))
            {
                if (other.GetComponent<ShieldScript>().player == sender)
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

            instance = Instantiate(HitEffect, transform.position, Quaternion.identity);
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
        StartCoroutine(Destroy());
    }


    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(20);
        Destroy(this.gameObject);
    }
}
