using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEffects : MonoBehaviour
{
    public Transform right;
    public Transform left;

    private Vector3 rPrevPos;
    private Vector3 lPrevPos;

    public GameObject trail;
    public float spacing = 0.7f;
    public Transform rotationAnchor;
    public bool emit = true;

    private Transform trails;


    private void Awake()
    {
        trails = GameObject.FindGameObjectWithTag("Trails").transform;
    }

    private void Update()
    {
        if (emit)
        {
            if ((rPrevPos - right.position).sqrMagnitude >= spacing * spacing || (lPrevPos - left.position).sqrMagnitude >= spacing * spacing)
            {
                rPrevPos = right.position;
                lPrevPos = left.position;
                GameObject instance;
                instance = Instantiate(trail, right.position, right.rotation);
                instance.transform.parent = trails;
                instance = Instantiate(trail, left.position, left.rotation);
                instance.transform.parent = trails;
            }
        }
    }
    
}
