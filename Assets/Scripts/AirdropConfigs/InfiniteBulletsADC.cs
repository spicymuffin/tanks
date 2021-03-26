using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteBulletsADC : MonoBehaviour
{
    public string type = "InfiniteBullets";
    public float time = 10f;
    public GameObject icon;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.GiveAirdrop(type, this.gameObject);
            Destroy(this.gameObject);
        }
    }
}
