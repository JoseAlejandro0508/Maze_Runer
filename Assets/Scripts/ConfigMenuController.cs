using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

using UnityEngine.SceneManagement;
public class ConfigMenuController : MonoBehaviour
{
    public GameObject AdvancedMenu;
    public Slider TrapsProbability;
    public Slider RewardsProbability;
    public TMP_Text TrapsProbInd;
    public TMP_Text RewardsProbInd;
    public TMP_Dropdown MapSizeDrop;
    public TMP_Dropdown PlayersNumDrop; 

    int[] MapSizes={15,23,31,35};

    void Start()
    {
        DefaultRewardTrapsProbability();

        DisplayIndicators();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AdvancedSettingsView(){
       AdvancedMenu.SetActive(true);

    }
    public void AdvancedApplySettings(){

        AdvancedMenu.SetActive(false);

    }
    public void SetRewardTrapsProbability(){

        PlayerPrefs.SetInt("TrapsProbability", (int)TrapsProbability.value);
        PlayerPrefs.SetInt("RewardsProbability", (int)RewardsProbability.value);
        DisplayIndicators();

    }
    public void DisplayIndicators(){
        TrapsProbInd.text=TrapsProbability.value.ToString();
        RewardsProbInd.text=RewardsProbability.value.ToString();
    }
    public void DefaultRewardTrapsProbability(){

        TrapsProbability.value=10;
        RewardsProbability.value=5;
        SetRewardTrapsProbability();



    }
    public void PlayersNumber()
    {   
        int index=PlayersNumDrop.value;
        // Obtener la opci�n seleccionada
        string selected = PlayersNumDrop.options[index].text;

        switch (selected)
        {
            case "1 Player":
                PlayerPrefs.SetInt("Number_of_Players", 1);
                Debug.Log("Opci�n seleccionada: " + selected);
                break;
            case "2 Players":
                PlayerPrefs.SetInt("Number_of_Players", 2);
                Debug.Log("Opci�n seleccionada: " + selected);
                break;
            case "3 Players":
                PlayerPrefs.SetInt("Number_of_Players", 3);
                Debug.Log("Opci�n seleccionada: " + selected);
                break;
            case "4 Players":
                PlayerPrefs.SetInt("Number_of_Players", 4);
                Debug.Log("Opci�n seleccionada: " + selected);
                break;
            case "5 Players":
                PlayerPrefs.SetInt("Number_of_Players", 5);
                Debug.Log("Opci�n seleccionada: " + selected);
                break;
            case "6 Players":
                PlayerPrefs.SetInt("Number_of_Players", 6);
                Debug.Log("Opci�n seleccionada: " + selected);
                break;
        }

    }
    public void ChangeScene(string sceneName)
    {
        Debug.Log("Cambiando a la escena: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
    public void ConfirmButton(){
        int selected_size = MapSizes[MapSizeDrop.value];
        PlayerPrefs.SetInt("MapSize", selected_size);
        PlayersNumber();
        ChangeScene("RolSelect");
    }


}
