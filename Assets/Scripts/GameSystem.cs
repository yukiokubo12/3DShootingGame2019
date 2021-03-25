using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public Button toMainButton;

    // public GameObject toMainButton;

    void Start()
    {

    }

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
        // toMainButton = GetComponent<
        this.toMainButton = GetComponent<Button>();
    }
}
