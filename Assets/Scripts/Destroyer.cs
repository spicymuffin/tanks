using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public AudioSource explosionSound;
    private float volumeValue = SoundManager.soundFloat;


    public void Awake()
    {
        explosionSound.volume = volumeValue;
        explosionSound.Play();
        Destroy();
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }


}
