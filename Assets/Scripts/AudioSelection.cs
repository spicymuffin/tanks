using System.Collections;
using UnityEngine;

public class AudioSelection : MonoBehaviour
{
    public AudioSource hittingWallSound1;
    public AudioSource hittingWallSound2;
    private float volumeValue = SoundManager.soundFloat;
    public System.Random rand = new System.Random();

    public void Choose()
    {
        hittingWallSound1.volume = volumeValue;
        hittingWallSound2.volume = volumeValue;
        int value = rand.Next(1, 3);
        if(value == 1)
        {
            //Debug.Log('1');
            hittingWallSound1.Play();
        }
        else
        {
            //Debug.Log('2');
            hittingWallSound2.Play();
        }
    }


    public void Awake()
    {
        Choose();
        StartCoroutine(Destroyer());
    }


    IEnumerator Destroyer()
    {
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }
}