using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public class QuitMenuController : MonoBehaviour
{
    public GameObject MapObject;
    public GameObject TutorialPanel;
    public GameObject MenuPanel;

    public void QuitClick(){
        Debug.Log("Quit");
        Application.Quit();
        
    }
    public void MainMenuClick(){
        Debug.Log("Menu");
        SceneManager.LoadScene("PlayersNumber");
    }
    public void RestartClick()
    {
        Debug.Log("Restart");
        SceneManager.LoadScene("Map1");
    }
    public void Enciclopedy()
    {
        Debug.Log("Restart");
        SceneManager.LoadScene("Map1");
    }
    public void Tutorial()
    {
        TutorialPanel.SetActive(!TutorialPanel.activeSelf);
        
    }
    public void OpenMenu(){
        MapObject.GetComponent<Map>().OnMenu=!MapObject.GetComponent<Map>().OnMenu;
        MenuPanel.SetActive(!MenuPanel.activeSelf);
        if(!MenuPanel.activeSelf)TutorialPanel.SetActive(false);
        

    }



}
