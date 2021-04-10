using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
    public MeshRenderer mr;
    private Material material;
    public float decaySpeed = 2f;
    private float initialRadius;
    public float tolerance = 0.0001f;
    public float target = -0.4f;
    private Coroutine cr;
    private void Awake()
    {
        material = mr.material;
        initialRadius = material.GetFloat("Vector1_RADIUS");
    }

    private void OnTriggerEnter(Collider collider)
    {
        material.SetVector("Vector3_CENTER", transform.InverseTransformPoint(collider.transform.position));
        material.SetFloat("Vector1_RADIUS", initialRadius);
        if (cr != null)
        {
            StopCoroutine(cr);
        }
        cr = StartCoroutine(LerpRadius());
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
