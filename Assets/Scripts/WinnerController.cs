using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Collections;
public class WinnerController : MonoBehaviour
{
    private int Player_ID;
    private string WinnerName;
    private string WinnerRole;


    public TMP_Text PlayerWinnerText; 

    public Sprite CABackground;
    public Sprite VisionBackground;
    public Sprite HEBackground;
    public Sprite IMBackground;
    public Sprite HulkBackground;
    public Sprite ThorBackground;
    public GameObject BackgroundDisplay;
    public GameObject WinnerMenu;
    Dictionary<string, Dictionary<string, Sprite>> Textures = new Dictionary<string, Dictionary<string, Sprite>>();

   
    void Start()
    {
        Player_ID= PlayerPrefs.GetInt("Winner");
        WinnerName= PlayerPrefs.GetString($"PlayerName_{Player_ID}");
        WinnerRole=PlayerPrefs.GetString($"PlayerRole_{Player_ID}");
        InitDbs();
        DisplayWinnerText();
        DisplayBackground();
        CargarMenu();

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void InitDbs()
    {
        Textures["Capitan America"] = new Dictionary<string, Sprite>
        {
            { "Background", CABackground },

        };
        Textures["Iron Man"] = new Dictionary<string, Sprite>
        {
            {  "Background", IMBackground },

        };
        Textures["Thor"] = new Dictionary<string, Sprite>
        {
            {  "Background", ThorBackground },

        };
        Textures["Vision"] = new Dictionary<string, Sprite>
        {
            {  "Background", VisionBackground },

        };
        Textures["Hawk Eye"] = new Dictionary<string, Sprite>
        {
            { "Background", HEBackground },

        };
        Textures["Hulk"] = new Dictionary<string, Sprite>
        {
            {  "Background",HulkBackground},

        };


    }
    public void DisplayWinnerText()
    {
        PlayerWinnerText.text = $"{WinnerName} WINS";
    }
    public void DisplayBackground()
    {
        Sprite BackgroundTexture = Textures[WinnerRole]["Background"];
    
        BackgroundDisplay.GetComponent<Image>().sprite = BackgroundTexture;

    }
    public void CargarMenu()
    {
        StartCoroutine(EsperarYcargarEscena());
    }

    private IEnumerator EsperarYcargarEscena()
    {
        yield return new WaitForSeconds(2f);
        WinnerMenu.SetActive(true);
    }

}
