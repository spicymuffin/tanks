using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAnimator : MonoBehaviour
{
    [Header("Assignables")]
    public Transform head;
    public Transform tip;
    public GameObject decorativeRocket;
    public Camera mcam;
    public ShieldScript shield;
    public AutogunScript autogun;
    public LandmineScript landmine;
    public AudioSource asource;

    [Header("Shield")]
    public float STimer = 25f;
    public float SDuration = 10f;

    [Header("Auto gun")]
    public float AGTimer = 15f;
    public int AGShots = 5;
    public float AGDelay = 0.3f;

    [Header("Landmine(s)")]
    public float LTimer = 12f;
    public float LTimeToArm = 0.4f;

    [Header("Head Rotation")]
    public float headRotationTime = 0.5f;

    public Vector3 tankpos;

    bool shootFlag = false;
    Player nullPlayer = new Player();

    Coroutine coroutine;

    void Update()
    {
        tankpos = mcam.WorldToScreenPoint(head.position);
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(DT(0.2f));
            Vector2 screentank = new Vector2(tankpos.x, tankpos.y);
            Vector2 dir = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - screentank;
            Vector3 dir3D = new Vector3(dir.x, 0, dir.y);
            Quaternion rot = Quaternion.LookRotation(Vector3.up, dir3D);
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(Rotate(head, rot));
            print(Input.mousePosition);
        }
    }

    private void Start()
    {
        StartCoroutine(S_Timer());
        StartCoroutine(AG_Timer());
        StartCoroutine(L_Timer());
    }

    public IEnumerator DT(float time)
    {
        float t = 0;
        shootFlag = false;
        yield return new WaitForEndOfFrame();
        while (t < time)
        {
            t += Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                shootFlag = true;
                break;
            }
            yield return null;
        }
    }

    public IEnumerator S_Timer()
    {
        yield return new WaitForSeconds(STimer);
        shield.Appear();
        yield return new WaitForSeconds(SDuration);
        shield.Disappear();
        StartCoroutine(S_Timer());
    }

    public IEnumerator AG_Timer()
    {
        yield return new WaitForSeconds(AGTimer);
        for (int i = 0; i < AGShots; i++)
        {
            yield return new WaitForSeconds(AGDelay);
            autogun.ShootDecorative();
        }
        StartCoroutine(AG_Timer());
    }

    public IEnumerator L_Timer()
    {
        yield return new WaitForSeconds(LTimer);
        landmine.Animate(LTimeToArm);
        StartCoroutine(L_Timer());
    } 

    public IEnumerator Rotate(Transform _object, Quaternion _target)
    {
        float t = 0.0f;

        Quaternion start = _object.rotation;
        Quaternion end = _target;

        while (t < headRotationTime)
        {
            t += Time.deltaTime;
            _object.rotation = Quaternion.Lerp(start, end, t / headRotationTime);
            yield return null;
        }

        if (shootFlag)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        GameObject currentRocket = Instantiate(decorativeRocket, tip.position, Quaternion.Euler(tip.rotation.eulerAngles.x, tip.rotation.eulerAngles.y, tip.rotation.eulerAngles.z));
        currentRocket.GetComponent<DecorativeRocket>().sender = nullPlayer;
        asource.Play();
    }
}
