using UnityEngine;

public class EnciclopedyController : MonoBehaviour
{
    public GameObject Enciclopedy;
    public GameObject PlayersEnc;
    public GameObject RewardsEnc;
    public GameObject CAInf;
    public GameObject HEInf;
    public GameObject HulkInf;
    public GameObject ThorInf;
    public GameObject VisionInf;
    public GameObject IMInf;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void RefreshPlayerEnc()
    {
        CAInf.SetActive(false);
        HEInf.SetActive(false);
        HulkInf.SetActive(false);
        ThorInf.SetActive(false);
        VisionInf.SetActive(false);
        IMInf.SetActive(false);
    }
    public void CAEnc(){
        RefreshPlayerEnc();
        CAInf.SetActive(true);
    }
    public void HEEnc(){
        RefreshPlayerEnc();
        HEInf.SetActive(true);
    }
    public void HulkEnc(){
        RefreshPlayerEnc();
        HulkInf.SetActive(true);
    }
    public void  ThorEnc(){
        RefreshPlayerEnc();
        ThorInf.SetActive(true);
    }
    public void VisionEnc(){
        RefreshPlayerEnc();
        VisionInf.SetActive(true);
    }
    public void IMEnc(){
        RefreshPlayerEnc();
        IMInf.SetActive(true);
    }
    public void OpenPlayersEnc(){
        PlayersEnc.SetActive(!PlayersEnc.activeSelf);
    }
    public void OpenEnc(){
        Enciclopedy.SetActive(!Enciclopedy.activeSelf);
    }

}

