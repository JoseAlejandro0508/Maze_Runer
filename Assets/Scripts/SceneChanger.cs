using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger:MonoBehaviour
{

    public void ChangeScene(string sceneName)
    {
        Debug.Log("Cambiando a la escena: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}

