using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;
    public GameObject pauseMenu;
    public GameObject pausebutton;


    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
        pausebutton.SetActive(false);
    }


    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
        pausebutton.SetActive(true);
    }


    public void GoToMainMenu()
    {
        ServerSend.DisconnectAll();
        Server.Stop();
        SceneManager.MoveGameObjectToScene(NetworkManager.instance.gameObject, SceneManager.GetActiveScene());
        SceneManager.MoveGameObjectToScene(GameManager.instance.gameObject, SceneManager.GetActiveScene());
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
