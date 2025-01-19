using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ParpadeoLuz : MonoBehaviour
{
    
    public float radioInicial = 0.5f;
    public float radioFinal = 1.2f;
    public float duracionParpadeo = 1.0f;

    private float tiempoParpadeo = 0.0f;
    public bool Active=false;
    public  bool parpadeando = true;

    void Update()
    {
        if(!Active)return;
        Light2D luz=GetComponent<Light2D>();
        tiempoParpadeo += Time.deltaTime;

        if (parpadeando)
        {
            luz.pointLightOuterRadius = Mathf.Lerp(radioInicial, radioFinal, tiempoParpadeo / duracionParpadeo);
        }
        else
        {
            luz.pointLightOuterRadius = Mathf.Lerp(radioFinal, radioInicial, tiempoParpadeo / duracionParpadeo);
        }

        if (tiempoParpadeo >= duracionParpadeo)
        {
            parpadeando = !parpadeando;
            tiempoParpadeo = 0.0f;
        }
    }
}