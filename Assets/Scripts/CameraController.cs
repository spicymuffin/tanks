using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerCamera;
    public float shakeSpeed = 37f;
    public float shakeDistance = 0.5f;
    public float shakeDuration = 0.25f;
    public float tolerance = 0.1f;
    private Vector3 normalPos;
    private Vector3 newPosition;
    private float time = 0;

    private void Awake()
    {
        normalPos = playerCamera.localPosition;
    }

    private IEnumerator Shake(float shakeSpeed, float shakeDistance, float shakeDuration, float tolerance, Vector3 normalPos)
    {
        while (time < shakeDuration)
        {
            if ((newPosition - playerCamera.localPosition).sqrMagnitude <= tolerance * tolerance)
            {
                newPosition = normalPos + Random.insideUnitSphere * shakeDistance;
            }
            playerCamera.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * shakeSpeed);
            time += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(ResetPosSmooth(normalPos));
        time = 0;
    }

    public void ShakeCamera()
    {
        StartCoroutine(Shake(shakeSpeed, shakeDistance, shakeDuration, tolerance, normalPos));
    }

    private IEnumerator ResetPosSmooth(Vector3 normalPos)
    {
        while (Vector3.Distance(playerCamera.transform.localPosition, normalPos) > 0.01f)
        {
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, normalPos, Time.deltaTime * 20f);
            yield return null;
        }
        playerCamera.localPosition = normalPos;
    }
}
