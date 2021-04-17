using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchChange : MonoBehaviour
{
    public AudioSource ricochetSound;
    public void Awake()
    {
        ricochetSound = GetComponent<AudioSource>();
        ricochetSound.pitch = Random.Range(1.0f, 1.15f);
        ricochetSound.Play();
    }
}
