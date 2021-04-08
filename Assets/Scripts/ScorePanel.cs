using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
    public Image bar;
    public float fillSpeed = 0.75f;
    public float moveSpeed = 5f;

    public void SetFill(float fill)
    {
        StartCoroutine(IE_SetFill(fill));
    }

    public void MoveTo(Vector3 target)
    {
        StartCoroutine(IE_MoveTo(target));
    }

    public IEnumerator IE_MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * moveSpeed);
            yield return null;
        }
    }

    public IEnumerator IE_SetFill(float fill)
    {
        while (fill - bar.fillAmount > 0.001f)
        {
            bar.fillAmount = Mathf.Lerp(bar.fillAmount, fill, fillSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
