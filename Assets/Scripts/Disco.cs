using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disco : MonoBehaviour
{
    private Light lightSource;
    public Gradient colorGradient;
    public float duration = 1f;

    private void Awake()
    {
        lightSource = GetComponent<Light>();
    }

    void Update()
    {
        float lerp = Mathf.PingPong(Time.time, duration) / duration;
        lightSource.color = colorGradient.Evaluate(lerp);
    }
}
