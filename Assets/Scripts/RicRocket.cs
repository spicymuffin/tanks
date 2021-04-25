using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RicRocket : MonoBehaviour
{
    [Header("General")]
    public float speed;
    public int maxDeflects = 3;
    [Header("Effects")]
    public GameObject sparks;
    public GameObject hitEffect;
    [Header("Sounds")]
    public GameObject ricochetTheWall;
    public GameObject hittingTheWall;
    //где звук удара об щит??
    [Header("Assignables")]
    public SelfDestruct smokeTrail;
    [HideInInspector]
    public Player sender;

    private bool flag = false;
    private int deflectCounter = 0;
    private void FixedUpdate()
    {
        transform.position += transform.forward * speed;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward * 0.1f);
        Physics.Raycast(ray, out hit);
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(transform.position, fwd, 1))
        {
            GameObject instance;
            if (deflectCounter != maxDeflects)
            {
                deflectCounter++;
                Vector3 reflectDir = Vector3.Reflect(ray.direction, hit.normal);
                float rot = 90 - Mathf.Atan2(reflectDir.z, reflectDir.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0, rot, 0);
                instance = Instantiate(sparks, hit.point, Quaternion.LookRotation(hit.normal));
                instance.transform.parent = LevelConfig.instance.effects;
                instance = Instantiate(ricochetTheWall);
                instance.transform.parent = LevelConfig.instance.effects;
            }
            else
            {
                instance = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                instance.transform.parent = LevelConfig.instance.effects;
                instance = Instantiate(hittingTheWall);
                instance.transform.parent = LevelConfig.instance.effects;
                smokeTrail.gameObject.transform.parent = LevelConfig.instance.effects;
                smokeTrail.Destroy();
                flag = true;
            }
            Transform parent = hit.collider.transform.parent;
            if (parent)
            {
                Player hitPlayer;
                if (hit.collider.transform.parent.TryGetComponent<Player>(out hitPlayer))
                {
                    if (hitPlayer == sender)
                    {
                        hitPlayer.BulletDie();
                        sender.kills--;
                    }
                    else
                    {
                        hitPlayer.BulletDie();
                        sender.kills++;
                    }
                    Destroy(this.gameObject);
                }
            }
            if (hit.collider.CompareTag("Barrel"))
            {
                hit.collider.GetComponent<ExplosiveBarrel>().Explode();
                smokeTrail.gameObject.transform.parent = LevelConfig.instance.effects;
                smokeTrail.Destroy();
                Destroy(this.gameObject);
            }
            if (hit.collider.CompareTag("Shield"))
            {
                hit.collider.GetComponent<ShieldScript>().player.shieldBlocks++;
            }
            if (flag)
            {
                Destroy(this.gameObject);
            }
        }

        //Hotta будто glo
    }
    private void Awake()
    {
        transform.parent = LevelConfig.instance.rockets;
    }
}