using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public GameObject MainCamera;
    public GameObject levelSelectingCameraPosition;
    public GameObject createGameCameraPosition;
    public GameObject mainMenuCameraPosition;
    public GameObject audioSliderCameraPosition;
    public AudioSource buttonSound;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        buttonSound = GetComponent<AudioSource>();

    }

    public void ButtonSound()
    {
        buttonSound.pitch = Random.Range(1.0f, 1.15f);
        buttonSound.Play();
    }

    public void MoveToStartGame()
    {
        StopAllCoroutines();
        StartCoroutine(LerpObject(createGameCameraPosition, MainCamera));
    }

    public void MoveToMainMenu()
    {
        StopAllCoroutines();
        StartCoroutine(LerpObject(mainMenuCameraPosition, MainCamera));
    }

    public void MoveToLevelSelecting()
    {
        StopAllCoroutines();
        StartCoroutine(LerpObject(levelSelectingCameraPosition, MainCamera));
    }

    public void MoveToAudioSlider()
    {
        StopAllCoroutines();
        StartCoroutine(LerpObject(audioSliderCameraPosition, MainCamera));
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator LerpObject(GameObject __target, GameObject _object)
    {
        while (Vector3.Distance(_object.transform.position, __target.transform.position) > 0.001f || Quaternion.Angle(_object.transform.rotation, __target.transform.rotation) > 0.01f)
        {
            _object.transform.position = Vector3.Lerp(_object.transform.position, __target.transform.position, Time.deltaTime * 5f);
            _object.transform.rotation = Quaternion.Lerp(_object.transform.rotation, __target.transform.rotation, Time.deltaTime * 5f);
            yield return new WaitForEndOfFrame();
        }
    }
}
