using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerLoader : MonoBehaviour
{
    private void Start()
    {
        AudioSource[] array = GetComponents<AudioSource>();
        
        foreach(AudioSource src in array)
        {
            src.volume = SoundManager.soundFloat;
        }
    }
}
