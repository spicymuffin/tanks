﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    public ParticleSystem smoke;
    public ParticleSystem flames;
    public ParticleSystem smallbois;

    private void Awake()
    {
        smoke.Play();
        flames.Play();
        smallbois.Play();
        Invoke(nameof(Destroy), 5f);
    }

    private void Destroy()
    {
        Destroy(this.gameObject);
    }
}