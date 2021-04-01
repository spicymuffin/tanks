using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider slider;
    public static float soundFloat;


    public void Start()
    {
        slider.value = PlayerPrefs.GetFloat("save", soundFloat);
    }

    public void SaveSoundSettings(float value)
    {
        soundFloat = value;
        PlayerPrefs.SetFloat("save", soundFloat); 
    }
}
