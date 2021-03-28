using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed;
    public GameObject HitEffect;
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
            if (other.CompareTag("Shield"))
            {
                if (other.GetComponent<ShieldScript>().player == sender)
                {
                    return;
                }
            }
            GameObject instance = Instantiate(HitEffect, transform.position, Quaternion.identity);
            smokeTrail.gameObject.transform.parent = GameObject.FindGameObjectWithTag("Effects").transform;
            smokeTrail.Destroy();
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
