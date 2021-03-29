using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public float radius;
    public float force;
    public float explosionDelay;
    private bool isExplosion;
    private LevelConfig levelConfig;
    public bool run = false;
    private Player player;
    private LayerMask ignoreMask;
    public LayerMask ignoreLineCast;
    public LayerMask ignoreRayCast;
    public GameObject explosionEffect;

    public void Awake()
    {
        ignoreMask = ignoreLineCast | ignoreRayCast;
        levelConfig = GameObject.FindGameObjectWithTag("LevelConfig").GetComponent<LevelConfig>();
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
        Invoke("Explode", explosionDelay);
        GetComponent<Renderer>().material.color = Color.red;
    }

    public void Explode()
    {
        CameraController.instance.Shake();
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < overlappedColliders.Length; i++)
        {
            Rigidbody rigidbody = overlappedColliders[i].attachedRigidbody;
            RaycastHit hitData;
            bool hit = Physics.Raycast(transform.position, overlappedColliders[i].transform.position - transform.position, out hitData, Vector3.Distance(overlappedColliders[i].transform.position, transform.position), ~ignoreMask);
            Debug.DrawRay(transform.position, overlappedColliders[i].transform.position - transform.position, Color.yellow, 60);
            
            if (hit)
            {
                if (hitData.collider.gameObject == overlappedColliders[i].gameObject)
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
                }

                GameObject inst = new GameObject();
                inst.name = $"{hitData.collider.gameObject == overlappedColliders[i].gameObject}: {overlappedColliders[i].name}";
                inst.transform.position = hitData.point;
            }
        }
        Destroy(gameObject);
    }
}
