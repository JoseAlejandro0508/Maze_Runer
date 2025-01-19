using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuController : MonoBehaviour
{
  

    public void Play(){
        SceneManager.LoadScene("PlayersNumber");
    }
    public void Quit(){
        Application.Quit();
    }
    
}
