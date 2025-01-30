using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public GameObject Movement;
    public GameObject Skills;
    public GameObject Turn;

    public GameObject TR;
    public GameObject Menu;
    public GameObject Map;
    public GameObject PP;
    public GameObject Health;
    public GameObject Speed;
    public GameObject Vision;
    public GameObject Status;
    public GameObject WC;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public  void RefreshStatus(){
        Movement.SetActive(false);
        Skills.SetActive(false);
        Turn.SetActive(false);
        TR.SetActive(false);
        Menu.SetActive(false);
        Map.SetActive(false);
        PP.SetActive(false);
        Health.SetActive(false);
        Speed.SetActive(false);
        Vision.SetActive(false);
        Status.SetActive(false);
        WC.SetActive(false);

    }
    public void MovementView(){
        RefreshStatus();
        Movement.SetActive(true);
    }
    public void SkillsView(){
        RefreshStatus();
        Skills.SetActive(true);
    }
    public void TurnView(){
        RefreshStatus();
        Turn.SetActive(true);
    }
    public void TRView(){
        RefreshStatus();
        TR.SetActive(true);
    }
    public void MenuView(){
        RefreshStatus();
        Menu.SetActive(true);
    }
    public void MapView(){
        RefreshStatus();
        Map.SetActive(true);
    }
    public void PPView(){
        RefreshStatus();
        PP.SetActive(true);
    }
    public void HealthView(){
        RefreshStatus();
        Health.SetActive(true);
    }
    public void SpeedView(){
        RefreshStatus();
        Speed.SetActive(true);
    }
    public void VisionView(){
        RefreshStatus();
        Vision.SetActive(true);
    }
    public void StatusView(){
        RefreshStatus();
        Status.SetActive(true);
    }
    public void WCView(){
        RefreshStatus();
        WC.SetActive(true);
    }
    



}
