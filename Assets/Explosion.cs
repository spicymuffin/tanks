using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Radius;
    public float Force;
    public float DestroyTime;
    public bool isExplosion;
    public bool run = false;
    public Player player;


    public void FixedUpdate()
    {
        GoRun();
    }
    public void GoRun()
    {
        if (run)
        {
            ExplodeDelay();
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket"))
        {
            ExplodeDelay();
        }
    }
    public void ExplodeDelay()
    {
        if (isExplosion) return;
        isExplosion = true;
        Invoke("Explode", 3f);
        GetComponent<Renderer>().material.color = Color.red;
    }

    public void Explode()
    {
        Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, Radius);
        for (int i = 0; i < overlappedColliders.Length; i++)
        {
            Rigidbody rigidbody = overlappedColliders[i].attachedRigidbody;

            if (overlappedColliders[i].TryGetComponent<Player>(out player) && Physics.Linecast(transform.position, overlappedColliders[i].transform.position))
            {
                Debug.LogError('a');
                player.ExplosionDie();
            }

            if (rigidbody)
            {
                rigidbody.AddExplosionForce(Force, transform.position, Radius);
                Explosion explosion = rigidbody.GetComponent<Explosion>();

                if (explosion)
                {
                    if (Vector3.Distance(transform.position, rigidbody.position) < Radius / 2f)
                    {
                        explosion.ExplodeDelay();
                    }
                }
            }
        }
        Destroy(gameObject);
    }
}
