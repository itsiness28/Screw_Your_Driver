using UnityEngine;
using TMPro;

public class Minigames : MonoBehaviour
{

    public int clickCount = 0;
    public int screwClicks = 10;
    public int screwsNeeded = 4;
    public TextMeshProUGUI ClickCountText;
    public TextMeshProUGUI ScrewsCountText;
    public bool chairBuilt = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScrewsCountText.text = ("You have " + screwsNeeded + " screws left.");
        ClickCountText.text = ("You have clicked " + clickCount + " times.");
    }

    // Update is called once per frame
    void Update()
    {

        Screws();
        EndGame();

    }


    public void OnImagePressed()
    {
        clickCount++;
        ClickCountText.text = ("You have clicked " + clickCount + " times.");
    }

    public void Screws ()
    {
        if (clickCount == 10 && screwsNeeded == 4)
        {
            screwsNeeded--;
            ScrewsCountText.text = ("You have " + screwsNeeded + " screws left.");
        }
        if (clickCount == 20 && screwsNeeded == 3)
        {
            screwsNeeded--;
            ScrewsCountText.text = ("You have " + screwsNeeded + " screws left.");
        }
        if (clickCount == 30 && screwsNeeded == 2)
        {
            screwsNeeded--;
            ScrewsCountText.text = ("You have " + screwsNeeded + " screws left.");
        }
        if (clickCount == 40 && screwsNeeded == 1)
        {
            screwsNeeded--;
            ScrewsCountText.text = ("You have " + screwsNeeded + " screws left.");
        }
    }

    public void EndGame()
    {
        if (clickCount >= 40 && screwsNeeded <= 0)
        {
            ScrewsCountText.text = ("Congrats!");
            ClickCountText.text = ("You built a chair.");
            chairBuilt = true;
        }
    }
}
