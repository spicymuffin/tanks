using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaDecay : MonoBehaviour
{
    public float decaySpeed = 30;
    public SpriteRenderer rend;

    private void Update()
    {
        float alpha = rend.color.a;
        float red = rend.color.r;
        float blu = rend.color.b;
        float grn = rend.color.g;

        Color color = new Color(red, grn, blu, alpha - decaySpeed * Time.deltaTime);
        rend.color = color;
    }
}
