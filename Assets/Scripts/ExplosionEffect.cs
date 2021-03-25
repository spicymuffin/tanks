using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public GameObject primaryEffect;
    public GameObject secondaryEffect;
    public GameObject smokeEffect;
    public GameObject smallBois;
    public GameObject debris;
    private ParticleSystem primary;
    private ParticleSystem secondary;
    private ParticleSystem smoke;
    private ParticleSystem small;
    private ParticleSystem deb;

    private void Awake()
    {
        primary = primaryEffect.GetComponent<ParticleSystem>();
        secondary = secondaryEffect.GetComponent<ParticleSystem>();
        smoke = smokeEffect.GetComponent<ParticleSystem>();
        small = smallBois.GetComponent<ParticleSystem>();
        deb = debris.GetComponent<ParticleSystem>();
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
