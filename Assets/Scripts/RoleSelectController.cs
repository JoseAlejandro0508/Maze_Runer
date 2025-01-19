using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RoleSelectController : MonoBehaviour
{
    private int Player_ID;
    private int NumberofPlayers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text Title; // Referencia al componente Text
    public TMP_Dropdown dropdown;
    public TMP_InputField input_field;
    public Button confirm_button;

    public Sprite CABackground;
    public Sprite VisionBackground;
    public Sprite HEBackground;
    public Sprite IMBackground;
    public Sprite HulkBackground;
    public Sprite ThorBackground;
    public GameObject BackgroundDisplay;
    Dictionary<string, Dictionary<string, Sprite>> Textures = new Dictionary<string, Dictionary<string, Sprite>>();

           
    void Start()
    {
        InitDbs();
        for (int i = 0; i < 6; i++)
        {
            PlayerPrefs.SetString($"PlayerRole_{i}", null);

        }
        NumberofPlayers =PlayerPrefs.GetInt("Number_of_Players", 1);
        confirm_button.onClick.AddListener(OnButtonClick);

        Player_ID = 0;

        Title.text = $"Select Player {Player_ID} Role";


        RefreshDropdownOptions();
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        DisplayBackground();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void RefreshDropdownOptions()
    {
        // Busca y elimina la opci�n espec�fica
        string role_sected;

        List<string> roles_selecteds = new List<string>();

        for (int i = 0; i < 6; i++)
        {
            role_sected= PlayerPrefs.GetString($"PlayerRole_{i}", null);
            if(role_sected!=null) roles_selecteds.Add(role_sected);


        }
        // Busca y elimina la opci�n espec�fica
        for (int i = dropdown.options.Count - 1; i >= 0; i--)
        {
            if (roles_selecteds.Contains(dropdown.options[i].text))
            {
                dropdown.options.RemoveAt(i);
            }
        }

        // Actualiza el dropdown para reflejar los cambios
        dropdown.RefreshShownValue();

    }
    void OnDropdownValueChanged(int index)
    {
        // Obtener la opci�n seleccionada
        string selected = dropdown.options[index].text;
        Debug.Log($"PlayerRole_{Player_ID}" + selected);
        DisplayBackground();

      
    }
    void OnButtonClick()

    {
        string selected_role = dropdown.options[dropdown.value].text;
        string player_name =input_field.text;
        PlayerPrefs.SetString($"PlayerRole_{Player_ID}", selected_role);
        PlayerPrefs.SetString($"PlayerName_{Player_ID}", player_name);
        Player_ID++;
        if (Player_ID <NumberofPlayers)

        {
            Title.text = $"Select Player {Player_ID} Role";

            Debug.Log("�Bot�n clickeado!");
            RefreshDropdownOptions();

        }
        else
        {

            SceneManager.LoadScene("Map1");

        }
        DisplayBackground();

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

    public void DisplayBackground()
    {
        string selected_role = dropdown.options[dropdown.value].text;
        Sprite BackgroundTexture = Textures[selected_role]["Background"];
    
        BackgroundDisplay.GetComponent<Image>().sprite = BackgroundTexture;

    }
    


}
