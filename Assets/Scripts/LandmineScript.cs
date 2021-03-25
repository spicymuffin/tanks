using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmineScript : MonoBehaviour
{
    public float timeToArm = 1.5f;
    private bool armed = false;
    public Material armedMaterial;
    public Player sender;
    private MeshRenderer mr;
    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        StartCoroutine(Arm());
    }

    private void OnTriggerStay(Collider other)
    {
        if (armed)
        {
            if (other.TryGetComponent<Player>(out Player hit))
            {
                if (hit != sender)
                {
                    sender.landmineKills++;
                    sender.kills++;
                }
                hit.LandmineDie();
                Destroy(this.gameObject);
            }
            if (other.gameObject.CompareTag("Airdrop"))
            {
                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
        }
    }

    IEnumerator Arm()
    {
        armed = false;
        yield return new WaitForSeconds(timeToArm);
        armed = true;
        mr.material = armedMaterial;
    }
}
