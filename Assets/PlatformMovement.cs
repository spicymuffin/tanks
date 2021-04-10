using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public float speed;

    public void Start()
    {
        Wait();
    }

    public void FixedUpdate()
    {
        transform.Translate(speed, 0, 0);
        
    }


    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
        speed = -speed;
        StartCoroutine(Wait());
    }
}
