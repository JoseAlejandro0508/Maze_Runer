using UnityEngine;
using TMPro;
public class WinnerController : MonoBehaviour
{
    private int Player_ID;
    private string WinnerName;
    private string WinnerRole;


    public TMP_Text PlayerWinnerText; 
   
    void Start()
    {
        Player_ID= PlayerPrefs.GetInt("Winner");
        WinnerName= PlayerPrefs.GetString($"PlayerName_{Player_ID}");
        WinnerRole=PlayerPrefs.GetString($"PlayerRole_{Player_ID}");
        DisplayWinnerText();

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void DisplayWinnerText()
    {
        PlayerWinnerText.text = $"{WinnerName} WINS";
    }

}
