using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldADC : MonoBehaviour
{
    public string type = "Shield";
    public float time = 7.69f;
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
