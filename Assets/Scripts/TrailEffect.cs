using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEffect : MonoBehaviour
{
    public float spacing;
    public GameObject trail;
    public Transform rotationAnchor;
    private GameObject trails;
    private Vector3 prevPos;
    public bool emit = true;

    private void Update()
    {
        if (emit)
        {
            if ((prevPos - transform.position).sqrMagnitude >= spacing * spacing)
            {
                prevPos = transform.position;
                GameObject instance = Instantiate(trail, transform.position, rotationAnchor.rotation);
                instance.transform.parent = trails.transform;
            }
        }
    }

    private void Awake()
    {
        prevPos = transform.position;
        trails = GameObject.FindGameObjectWithTag("Trails");
    }

    public void DeleteTrails()
    {
        
    }
}
