using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class QuitMenuController : MonoBehaviour
{
    

    public void QuitClick(){
        Debug.Log("Quit");
        Application.Quit();
        
    }
    public void MainMenuClick(){
        Debug.Log("kMenu");
        SceneManager.LoadScene("PlayersNumber");
    }
    public void RestartClick()
    {
        Debug.Log("Restart");
        SceneManager.LoadScene("Map1");
    }



}
