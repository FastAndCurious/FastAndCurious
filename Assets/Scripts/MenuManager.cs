using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{

    public GameObject MainMenuHolder;
    public GameObject CreditsHolder;

    public void Play()
    {
        Application.LoadLevel(1);
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
