using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public float radius;
    public float force;
    public float destroyDelay;
    private bool isExplosion;
    public bool run = false;
    private Player player;
    private LayerMask ignoreMask;
    public LayerMask ignoreLineCast;
    public LayerMask ignoreRayCast;

    public void Awake()
    {
        ignoreMask = ignoreMask | ignoreRayCast;
    }

    public void FixedUpdate()
    {
        Run();
    }
    public void Run()
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
        Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < overlappedColliders.Length; i++)
        {
            Rigidbody rigidbody = overlappedColliders[i].attachedRigidbody;
            RaycastHit hitData;
            bool hit = Physics.Linecast(transform.position, overlappedColliders[i].transform.position, out hitData, ~ignoreMask);
            if (!hit)
            {
                if (rigidbody)
                {
                    rigidbody.AddExplosionForce(force, transform.position, radius);

                    if (overlappedColliders[i].TryGetComponent<Player>(out player))
                    {
                        player.ExplosionDie();
                    }

                    ExplosiveBarrel barrel = rigidbody.GetComponent<ExplosiveBarrel>();
                    if (barrel)
                    {
                        if (Vector3.Distance(transform.position, rigidbody.position) < radius / 2f)
                        {
                            barrel.ExplodeDelay();
                        }
                    }

                }
                    Debug.Log(overlappedColliders[i].gameObject.name);
            }

        }
        Destroy(gameObject);
    }
}
