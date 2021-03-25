using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public Button toMainButton;

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }
    public void ToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void ShowMainButton()
    {
        this.toMainButton = GetComponent<Button>();
    }
}
