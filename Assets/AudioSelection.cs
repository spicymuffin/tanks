using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioSelection : MonoBehaviour
{
    public AudioSource hittingWallSound1;
    public AudioSource hittingWallSound2;
    public System.Random rand = new System.Random();

    public void TheChoice()
    {
        int value = rand.Next(1, 3);
        if(value == 1)
        {
            Debug.Log('1');
            hittingWallSound1.Play();
        }
        else
        {
            Debug.Log('2');
            hittingWallSound2.Play();
        }
    }


    public void Awake()
    {
        TheChoice();
        StartCoroutine(Destroyer());
    }


    IEnumerator Destroyer()
    {
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }
}