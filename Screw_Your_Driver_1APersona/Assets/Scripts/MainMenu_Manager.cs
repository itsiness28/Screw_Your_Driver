using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class MainMenu_Manager : MonoBehaviour
{
    [SerializeField] public Button startGame;
    [SerializeField] public Button optionsMenu;
    [SerializeField] public Button exitGame;

    public void NewGame()
    {
        SceneManager.LoadScene("Main_Game");
    }

    public void OptionsMenu()
    {
        SceneManager.LoadScene("Options_Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
