using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCallBox : MonoBehaviour
{
    public Player player;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Rocket>(out Rocket rocket))
        {
            if (rocket.sender != player) player.closeCalls++;
        }
    }
}
