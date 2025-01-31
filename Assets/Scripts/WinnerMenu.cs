using UnityEngine;
using UnityEngine.SceneManagement;
public class WinnerMenu : MonoBehaviour
{

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
}
