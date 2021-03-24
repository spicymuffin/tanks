using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RicRocket : MonoBehaviour
{
    public float speed;
    public Player sender;
    public int maxDeflects = 2;

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
            if (deflectCounter != maxDeflects)
            {
                deflectCounter++;
                Vector3 reflectDir = Vector3.Reflect(ray.direction, hit.normal);
                float rot = 90 - Mathf.Atan2(reflectDir.z, reflectDir.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0, rot, 0);
            }
            else
            {
                Destroy(this.gameObject);
            }
            if (hit.collider.CompareTag("Player"))
            {
                Player hitPlayer;
                hitPlayer = hit.collider.GetComponent<Player>();
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


    }

}