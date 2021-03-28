using System.Collections;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public bool destroyOnAwake = false;
    public float destroyAfter = 1f;
    
    public void Destroy()
    {
        StartCoroutine(Destroy_coroutine(destroyAfter));
    }


    public void Destroy(float time)
    {
        StartCoroutine(Destroy_coroutine(time));
    }

    private void Awake()
    {
        if (destroyOnAwake)
        {
            Destroy();
        }
    }
    IEnumerator Destroy_coroutine(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
