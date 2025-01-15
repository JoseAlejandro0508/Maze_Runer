using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button QuitButton;
    public Button MainMenu;
    public Button Restart;
    void Start()
    {
        QuitButton.onClick.AddListener(QuitClick);
        MainMenu.onClick.AddListener(MainMenuClick);
        Restart.onClick.AddListener(RestartClick);
        
    }
    public void QuitGame(){
        Application.Quit();
    }
    public void QuitClick(){
        Debug.Log("Click");
        QuitGame();
    }
    public void MainMenuClick(){
        SceneManager.LoadScene("PlayersNumber");
    }
    public void RestartClick(){
        SceneManager.LoadScene("Map1");
    }



}
