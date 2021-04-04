using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public ParticleSystem primary;
    public ParticleSystem secondary;
    public ParticleSystem smoke;
    public ParticleSystem small;
    public ParticleSystem deb;

    private void Awake()
    {
        transform.parent = LevelConfig.instance.effects;
        primary.Play();
        secondary.Play();
        smoke.Play();
        small.Play();
        deb.Play();
        Invoke(nameof(Destroy), 5f);
    }

    private void Destroy()
    {
        Destroy(this.gameObject);
    }
}
