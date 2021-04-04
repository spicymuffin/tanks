using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEffect : MonoBehaviour
{
    public float spacing;
    public GameObject trail;
    public Transform rotationAnchor;
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
                instance.transform.parent = LevelConfig.instance.trails;
            }
        }
    }

    private void Awake()
    {
        prevPos = transform.position;
    }

    public void DeleteTrails()
    {
        
    }
}
