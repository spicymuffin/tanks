using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RicochetADC : MonoBehaviour
{
    public string type = "Ricochet";
    public float time = 6.69f;
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
