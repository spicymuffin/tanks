using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public float radius;
    public float force;
    public bool run = false;
    private LayerMask ignoreMask;
    public LayerMask ignoreExplosion;
    public GameObject explosionEffect;
    public GameObject explosionSound;

    public void Awake()
    {
        ignoreMask = ignoreExplosion;
    }

    public void FixedUpdate()
    {
        Run();
    }

    public void Run()
    {
        if (run)
        {
            Explode();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket"))
        {
            Explode();
        }
    }

    public void Explode()
    {
        Player player;
        GameObject instance = Instantiate(explosionSound);
        instance.transform.parent = LevelConfig.instance.effects;
        CameraController.instance.Shake();
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, radius, ~ignoreExplosion);
        for (int i = 0; i < overlappedColliders.Length; i++)
        {
            Rigidbody rb = overlappedColliders[i].attachedRigidbody;
            RaycastHit hitData;
            bool hit = Physics.Raycast(transform.position, overlappedColliders[i].transform.position - transform.position, out hitData, Vector3.Distance(overlappedColliders[i].transform.position, transform.position), ~ignoreMask);
            //Debug.DrawRay(transform.position, overlappedColliders[i].transform.position - transform.position, Color.yellow, 60);
            if (hit)
            {
                if (hitData.collider.gameObject == overlappedColliders[i].gameObject)
                {
                    if (rb)
                    {
                        if (rb.TryGetComponent<Player>(out player))
                        {
                            if (!player.isShielded)
                            {
                                player.ExplosionDie();
                            }
                        }
                        else
                        {
                            rb.AddExplosionForce(force, transform.position, radius);
                        }

                        ExplosiveBarrel barrel = rb.GetComponent<ExplosiveBarrel>();
                        if (barrel)
                        {
                            barrel.Explode();
                        }

                    }
                }

                //GameObject inst = new GameObject();
                //inst.name = $"{hitData.collider.gameObject == overlappedColliders[i].gameObject}: {overlappedColliders[i].name}";
                //inst.transform.position = hitData.point;
            }
        }
        Destroy(gameObject);
    }
}
