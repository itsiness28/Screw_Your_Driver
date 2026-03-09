using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class MainMenu_Manager : MonoBehaviour
{
    
   

     [SerializeField] bool pasar_nivel;
     [SerializeField] int indice_nivel;

     void Update()
    {
        if (pasar_nivel)
        {
            CambiarNivel(indice_nivel);
        }

    }

    public void CambiarNivel(int indice)
    {
        SceneManager.LoadScene(indice);
    }

  
    public void ExitGame()
    {
        Application.Quit();
    }


    //}
}
