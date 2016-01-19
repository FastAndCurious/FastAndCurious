using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public GameObject MainMenuHolder;
    public GameObject CreditsHolder;

    public void Play()
    {
        SceneManager.LoadScene("Main scene");
    }

    public void Credits()
    {
        MainMenuHolder.SetActive(false);
        CreditsHolder.SetActive(true);
    }

    public void MainMenu()
    {
        CreditsHolder.SetActive(false);
        MainMenuHolder.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
