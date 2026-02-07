using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShiftController : MonoBehaviour
{

    public bool shiftOn = false;
    [SerializeField] public float timerVar;
    [SerializeField] public GameObject canvasShiftOverview;
    [SerializeField] public Button closeShiftOverview;

    public TextMeshProUGUI shiftCountdown;
    public TextMeshProUGUI uiShiftIndicator;


    // Start is called before the first frame update
    void Start()
    {
        uiShiftIndicator.text = "Shop Closed";
    }

    // Update is called once per frame
    void Update()
    {
        UIShiftChange();
        //closeShiftOverview.onClick.AddListener(HideShiftOverview); //esto no anda bien

    }


    private void UIShiftChange()
    {
        if (shiftOn)
        {
            uiShiftIndicator.text = "Shop Open";
            Temporizador();
            if (timerVar == 0)
            {
                timerVar = 90;
            }
        }
        else
        {
            uiShiftIndicator.text = "Shop Closed";
            //canvasShiftOverview.SetActive(true);//esto tengo que revisarlo para que la primera vez no salga e incluirle un objeto, una imagen, que al ahcer click en ella vuelva a ocultarse.
        }
    }

    private void Temporizador()
    {
        timerVar -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(timerVar / 60);
        int seconds = Mathf.FloorToInt(timerVar % 60);
        shiftCountdown.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timerVar <= 0)
        {
            timerVar = 0;
            shiftCountdown.text = "0:00";
            shiftOn = false;
        }
    }

    private void HideShiftOverview()
    {
        canvasShiftOverview.SetActive(false);
    }


}
