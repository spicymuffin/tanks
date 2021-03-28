using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    public Player player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket"))
        {
            if(other.GetComponent<Rocket>().sender != player)
            {
                player.shieldBlocks++;
            }
        }
    }
}
