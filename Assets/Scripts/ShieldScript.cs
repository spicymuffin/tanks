using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    public Player player;
    public MeshRenderer mr;
    private Material material;
    public float decaySpeed = 2f;
    private float initialRadius;
    public float tolerance = 0.0001f;
    public float target = -0.4f;
    public float ATarget = -0.1f;
    public float ADuration = 1.5f;
    float AInit;
    private Coroutine cr;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket"))
        {
            if(other.GetComponent<Rocket>().sender != player)
            {
                player.shieldBlocks++;
            }
            material.SetVector("Vector3_CENTER", transform.InverseTransformPoint(other.transform.position));
            material.SetFloat("Vector1_RADIUS", initialRadius);
            if (cr != null)
            {
                StopCoroutine(cr);
            }
            cr = StartCoroutine(LerpRadius());
        }
    }
    private void Awake()
    {
        material = mr.material;
        initialRadius = material.GetFloat("Vector1_RADIUS");
        AInit = material.GetFloat("Vector1_DSSLV");
    }

    public void Appear()
    {
        StartCoroutine(LerpAppearance());
    }

    public void Disappear()
    {
        StartCoroutine(LerpDisappearance());
    }



    public IEnumerator LerpAppearance()
    {
        float t = 0.0f;
        float start = AInit;
        float end = ATarget;

        while (t < ADuration)
        {
            t += Time.deltaTime;
            material.SetFloat("Vector1_DSSLV", Mathf.Lerp(start, end, t / ADuration));
            yield return null;
        }
    }

    public IEnumerator LerpDisappearance()
    {
        float t = 0.0f;
        float start = material.GetFloat("Vector1_DSSLV");
        float end = AInit;

        while (t < ADuration)
        {
            t += Time.deltaTime;
            material.SetFloat("Vector1_DSSLV", Mathf.Lerp(start, end, t / ADuration));
            yield return null;
        }
    }

    public IEnumerator LerpRadius()
    {
        while (Mathf.Abs(material.GetFloat("Vector1_RADIUS") - target) > tolerance)
        {
            material.SetFloat("Vector1_RADIUS", Mathf.Lerp(material.GetFloat("Vector1_RADIUS"), target, decaySpeed * Time.deltaTime));
            yield return null;
        }
    }
}
