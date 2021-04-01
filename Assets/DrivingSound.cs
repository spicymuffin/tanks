using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingSound : MonoBehaviour
{
    public AudioSource drivingSound;
    public Rigidbody rb;

    public void FixedUpdate()
    {
        var vel = rb.velocity;
        var speed = vel.magnitude;
        //drivingSound.pitch = Mathf.Clamp(speed / 2, 1f, 2f);
    }
}
