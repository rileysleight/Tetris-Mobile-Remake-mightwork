using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenu;
    

    void Awake()
    {
        pauseMenu.SetActive(false);
    }

    public void GamePause()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void GameOver()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }
}