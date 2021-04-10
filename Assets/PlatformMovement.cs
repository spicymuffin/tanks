using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public float startMoveDuration;
    public float endMoveDuration;
    public GameObject target;

    public void Start()
    {
        Transform lol = target.GetComponent<Transform>();
        StartCoroutine(MoveCoroutine(lol));
    }


    IEnumerator MoveCoroutine(Transform target)
    {
        float t = 0.0f;
        float time = 0.0f;
        Vector3 start = transform.position;
        Vector3 end = target.position;

        while(true)
        {
            while (t < startMoveDuration)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(start, end, t / startMoveDuration);
                yield return null;
            }
            while (time < endMoveDuration)
            {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(end, start, time / endMoveDuration);
                yield return null;
            }
            t = 0.0f;
            time = 0.0f;
        }

    }
}
