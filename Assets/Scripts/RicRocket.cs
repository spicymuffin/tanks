using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RicRocket : MonoBehaviour
{
    public float speed;
    public Player sender;
    public int maxDeflects = 3;
    public GameObject sparks;
    public GameObject hitEffect;
    public GameObject ricochetTheWall;
    public GameObject hittingTheWall;
    public SelfDestruct smokeTrail;

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
                Destroy(this.gameObject);
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
                        Debug.Log($"killed: {hitPlayer.username}");
                        sender.kills--;
                    }
                    else
                    {
                        hitPlayer.BulletDie();
                        Debug.Log($"killed: {hitPlayer.username}");
                        sender.kills++;
                    }
                }
            }
            if (hit.collider.CompareTag("Barrel"))
            {
                hit.collider.GetComponent<ExplosiveBarrel>().Explode();
                smokeTrail.gameObject.transform.parent = LevelConfig.instance.effects;
                smokeTrail.Destroy();
                Destroy(this.gameObject);
            }
        }

        //Hotta будто glo
    }

}