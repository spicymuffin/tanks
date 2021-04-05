using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparks : MonoBehaviour
{
    public ParticleSystem smoke;
    public ParticleSystem smallbois;

    private void Awake()
    {
        transform.parent = LevelConfig.instance.effects;
        smoke.Play();
        smallbois.Play();
        Invoke(nameof(Destroy), 5f);
    }

    private void Destroy()
    {
        Destroy(this.gameObject);
    }
}
