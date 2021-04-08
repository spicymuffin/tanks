using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider slider;
    public static float soundFloat = 0.5f;
    public AudioSource sound;


    public void Start()
    {
        slider.value = PlayerPrefs.GetFloat("save", soundFloat);
        sound.mute = true;
    }


    public void SaveSoundSettings(float value)
    {
        soundFloat = value;
        PlayerPrefs.SetFloat("save", soundFloat);
        sound.volume = soundFloat;
        sound.mute = false;
        sound.Play();
    }
}
