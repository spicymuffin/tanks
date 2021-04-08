using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMovement : MonoBehaviour
{
    public new Transform transform;
    public float rotationAngle;
    public void FixedUpdate()
    {
        transform.Rotate(new Vector3(rotationAngle, 0, 0) * Time.deltaTime);
    }
}
