using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager2 : MonoBehaviour
{
    public void QuitApp()
    {
        Application.Quit();
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("LevelManager");
    }
}
