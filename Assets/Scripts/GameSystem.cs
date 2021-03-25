using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public Button toMainButton;
    public Button toTitleButton;
    public Text clearText;

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
        this.toMainButton.gameObject.SetActive(true);
    }
    public void ShowTitleButton()
    {
        this.toTitleButton.gameObject.SetActive(true);
    }
    public void ShowClearText()
    {
        this.clearText.GetComponent<Text>().text = "Game Clear!!";
        this.clearText.gameObject.SetActive(true);
    }
}
