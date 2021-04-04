using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmineScript : MonoBehaviour
{
    [Header("Main")]
    public float timeToArm = 3f;
    public float radius = 5f;
    public float force = 500f;
    public float falloff = 0.9f;

    [Header("Assignables")]
    public MeshRenderer mr;
    public Material armedMaterial;
    public Material normalMaterial;
    public GameObject explosionEffect;
    public GameObject explosionSound;
    public LayerMask ignoreExplosion;

    [Header("Public variables")]
    public Player creator;

    
    private LayerMask ignoreMask;
    float elapsedTime = 0f;
    float nextTime = 0f;
    bool armed = false;

    private void Awake()
    {
        ignoreMask = ignoreExplosion;
    }
    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= nextTime)
        {
            timeToArm = timeToArm * falloff;
            nextTime += timeToArm;
            FlipMat();
            if(timeToArm < 0.02f)
            {
                Explode();
            }
        }
    }

    public void Explode()
    {
        Player player;
        CameraController.instance.Shake();

        GameObject instance = Instantiate(explosionSound);
        instance.transform.parent = LevelConfig.instance.effects;

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


    private void FlipMat()
    {
        if (armed)
        {
            mr.material = normalMaterial;
            armed = false;
        }
        else
        {
            mr.material = armedMaterial;
            armed = true;
        }
    }
}
