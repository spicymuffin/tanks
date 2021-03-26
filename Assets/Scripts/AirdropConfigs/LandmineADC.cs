using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmineADC : MonoBehaviour
{
    public string type = "LandMine";
    public bool randomizeCount = true;
    public int count;
    public List<GameObject> icon;
    private void Start()
    {
        if (randomizeCount)
        {
            count = Random.Range(1, 4);
        }
    }

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
