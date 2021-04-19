using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAnimator : MonoBehaviour
{
    public Transform head;
    public Camera mcam;

    public float time;

    public Vector3 tankpos;

    Vector3 lastTouch;

    void Update()
    {
        tankpos = mcam.WorldToScreenPoint(head.position);
        print(tankpos);
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 screentank = new Vector2(tankpos.x, tankpos.y);


            Vector2 dir = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - screentank;

            Vector3 dir3D = new Vector3(dir.x, 0, dir.y);

            Quaternion rot = Quaternion.LookRotation(dir3D, -Vector3.up);

            //StartCoroutine(Rotate(head, rot));

            print(Input.mousePosition);

        }
        print(Input.mousePosition);
    }

    public IEnumerator Rotate(Transform _object, Quaternion _target)
    {
        float t = 0.0f;

        Quaternion start = _object.rotation;
        Quaternion end = _target;

        while (t < time)
        {
            t += Time.deltaTime;
            _object.rotation = Quaternion.Lerp(start, end, t / time);
            yield return null;
        }
    }
}
