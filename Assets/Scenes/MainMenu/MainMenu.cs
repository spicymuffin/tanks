using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public GameObject MainCamera;
    public GameObject LevelSelectingCameraPosition;
    public GameObject CreateGameCameraPosition;
    public GameObject MainMenuCameraPosition;

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
    }

    public void MoveToStartGame()
    {
        StopAllCoroutines();
        StartCoroutine(LerpObject(CreateGameCameraPosition, MainCamera));
    }

    public void MoveToMainMenu()
    {
        StopAllCoroutines();
        StartCoroutine(LerpObject(MainMenuCameraPosition, MainCamera));
    }

    public void MoveToLevelSelecting()
    {
        StopAllCoroutines();
        StartCoroutine(LerpObject(LevelSelectingCameraPosition, MainCamera));
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
