using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider slider;
    public static float soundFloat = 0.5f;
    public static int saveShots;

    public void Start()
    {
        slider.value = PlayerPrefs.GetFloat("save", soundFloat);
        //saveShots = PlayerPrefs.GetInt("saveShots", saveShots);
    }

    public void SaveSoundSettings(float value)
    {
        soundFloat = value;
        PlayerPrefs.SetFloat("save", soundFloat);
        //Debug.Log(saveShots);
    }
}
