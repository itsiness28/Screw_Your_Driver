using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class MainMenu_Manager : MonoBehaviour
{
    [SerializeField] public Button startGame;
    [SerializeField] public Button optionsMenu;
    [SerializeField] public Button exitGame;
    public static SceneManager instance;
    [SerializeField] Animator RawImage;


    //public void Start()
    //{
    //    RawImage.SetTrigger("Start");
    //}

    public void NewGame()
    {
        SceneManager.LoadScene("Main_Game");
        //RawImage.SetTrigger("End");
    }

    public void OptionsMenu()
    {
        SceneManager.LoadScene("Options_Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    //IEnumerator LoadGame()
    //{


      //  yield return new WaitForSeconds(1);
        //SceneManager.LoadScene("Main_Game");
        //RawImage.SetTrigger("Start");
    //}
}
