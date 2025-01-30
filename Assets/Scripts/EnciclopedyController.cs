using UnityEngine;

public class EnciclopedyController : MonoBehaviour
{
    public GameObject Enciclopedy;
    public GameObject PlayersEnc;
    public GameObject RewardsEnc;
    public GameObject Vmore;
    public GameObject Smore;
    public GameObject Hmore;
    public GameObject RH;
    public GameObject LD;
    public GameObject HD;
    public GameObject RP;
    public GameObject Vless;
    public GameObject Sless;

    public GameObject TrapsEnc;
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
    public void RefreshTrapsEnc()
    {
        Vless.SetActive(false);
        Sless.SetActive(false);
        LD.SetActive(false);
        HD.SetActive(false);
        RP.SetActive(false);

    }
    public void RefreshRewardsEnc()
    {
        Vmore.SetActive(false);
        Smore.SetActive(false);
        Hmore.SetActive(false);
        RH.SetActive(false);


    }
    public void CAEnc()
    {
        RefreshPlayerEnc();
        CAInf.SetActive(true);
    }
    public void HEEnc()
    {
        RefreshPlayerEnc();
        HEInf.SetActive(true);
    }
    public void HulkEnc()
    {
        RefreshPlayerEnc();
        HulkInf.SetActive(true);
    }
    public void ThorEnc()
    {
        RefreshPlayerEnc();
        ThorInf.SetActive(true);
    }
    public void VisionEnc()
    {
        RefreshPlayerEnc();
        VisionInf.SetActive(true);
    }
    public void IMEnc()
    {
        RefreshPlayerEnc();
        IMInf.SetActive(true);
    }
    public void OpenPlayersEnc()
    {
        PlayersEnc.SetActive(!PlayersEnc.activeSelf);
    }
    public void OpenEnc()
    {
        Enciclopedy.SetActive(!Enciclopedy.activeSelf);
    }
    public void OpenRewardEnc()
    {
        RewardsEnc.SetActive(!RewardsEnc.activeSelf);
    }
    public void OpenTrapsEnc()
    {
        TrapsEnc.SetActive(!TrapsEnc.activeSelf);
    }
    public void LDtrapEnc()
    {
        RefreshTrapsEnc();
        LD.SetActive(true);
    }
    public void HDtrapEnc()
    {
        RefreshTrapsEnc();
        HD.SetActive(true);
    }
    public void VlesstrapEnc()
    {
        RefreshTrapsEnc();
        Vless.SetActive(true);
    }
    public void SlesstrapEnc()
    {
        RefreshTrapsEnc();
        Sless.SetActive(true);
    }
    public void RestartrapEnc()
    {
        RefreshTrapsEnc();
        RP.SetActive(true);
    }
    public void VmoreEnc()
    {
        RefreshRewardsEnc();
        Vmore.SetActive(true);
    }
        public void SmoreEnc()
    {
        RefreshRewardsEnc();
        Smore.SetActive(true);
    }
        public void RHEnc()
    {
        RefreshRewardsEnc();
        RH.SetActive(true);
    }
        public void HmoreEnc()
    {
        RefreshRewardsEnc();
        Hmore.SetActive(true);
    }
    

}

