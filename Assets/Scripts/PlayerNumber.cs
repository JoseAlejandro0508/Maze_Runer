using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNumber: MonoBehaviour
{
    public TMP_Dropdown dropdown; // Asigna tu Dropdown desde el Inspector

    void Start()
    {
   
        // Asegúrate de que el Dropdown tenga al menos una opción
        if (dropdown.options.Count > 0)
        {
            // Obtener la opción seleccionada al inicio
            string selected = dropdown.options[dropdown.value].text;

          
            switch (selected)
            {
                case "1 Player":
                    PlayerPrefs.SetInt("Number_of_Players", 1);
                    break;
                case "2 Players":
                    PlayerPrefs.SetInt("Number_of_Players", 2);
                    break;
                case "3 Players":
                    PlayerPrefs.SetInt("Number_of_Players", 3);
                    break;
                case "4 Players":
                    PlayerPrefs.SetInt("Number_of_Players", 4);
                    break;
                case "5 Players":
                    PlayerPrefs.SetInt("Number_of_Players", 5);
                    break;
                case "6 Players":
                    PlayerPrefs.SetInt("Number_of_Players", 6);
                    break;
            }
            


        }

        // Añadir listener para detectar cambios en la selección
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void OnDropdownValueChanged(int index)
    {
        // Obtener la opción seleccionada
        string selected = dropdown.options[index].text;

        switch (selected)
        {
            case "1 Player":
                PlayerPrefs.SetInt("Number_of_Players", 1);
                Debug.Log("Opción seleccionada: " + selected);
                break;
            case "2 Players":
                PlayerPrefs.SetInt("Number_of_Players", 2);
                Debug.Log("Opción seleccionada: " + selected);
                break;
            case "3 Players":
                PlayerPrefs.SetInt("Number_of_Players", 3);
                Debug.Log("Opción seleccionada: " + selected);
                break;
            case "4 Players":
                PlayerPrefs.SetInt("Number_of_Players", 4);
                Debug.Log("Opción seleccionada: " + selected);
                break;
            case "5 Players":
                PlayerPrefs.SetInt("Number_of_Players", 5);
                Debug.Log("Opción seleccionada: " + selected);
                break;
            case "6 Players":
                PlayerPrefs.SetInt("Number_of_Players", 6);
                Debug.Log("Opción seleccionada: " + selected);
                break;
        }

    }
}
