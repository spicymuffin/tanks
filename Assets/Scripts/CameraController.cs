using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public Transform camera;
    public float shakeSpeed = 40f;
    public float shakeDistance = 0.7f;
    public float shakeDuration = 0.25f;
    public float tolerance = 0.3f;
    private Vector3 normalPos;
    private Vector3 newPosition;
    private float time = 0;

    private void Awake()
    {
        normalPos = camera.localPosition;
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private IEnumerator ShakeCoroutine(float shakeSpeed, float shakeDistance, float shakeDuration, float tolerance, Vector3 normalPos)
    {
        while (time < shakeDuration)
        {
            if ((newPosition - camera.localPosition).sqrMagnitude <= tolerance * tolerance)
            {
                newPosition = normalPos + Random.insideUnitSphere * shakeDistance;
            }
            camera.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * shakeSpeed);
            time += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(ResetPosSmooth(normalPos));
        time = 0;
    }

    public void Shake()
    {
        StartCoroutine(ShakeCoroutine(shakeSpeed, shakeDistance, shakeDuration, tolerance, normalPos));
    }

    private IEnumerator ResetPosSmooth(Vector3 normalPos)
    {
        while (Vector3.Distance(camera.transform.localPosition, normalPos) > 0.01f)
        {
            camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, normalPos, Time.deltaTime * 20f);
            yield return null;
        }
        camera.localPosition = normalPos;
    }
}
