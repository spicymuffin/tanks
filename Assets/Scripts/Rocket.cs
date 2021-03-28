using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed;
    public Player sender;
    public AudioSource hittingTheWallSound1;
    public AudioSource hittingTheWallSound2;
    public Renderer rend;
    public System.Random rand = new System.Random();
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
            int value = rand.Next(1, 3);
            if (value == 1)
            {
                hittingTheWallSound1.Play();
            }
            else
            {
                hittingTheWallSound2.Play();
            }
            StartCoroutine(Wait());
        }
    }
    private void Awake()
    {
        StartCoroutine(Destroy());

    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(20);
        Destroy(this.gameObject);
    }

    IEnumerator Wait()
    {

        rend.enabled = false;
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }

       
}
