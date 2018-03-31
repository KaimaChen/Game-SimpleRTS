using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager Instance = null;

    private Button mBtnPause;
    private Button mBtnRobot;
    private GameObject mBasePage;
    private GameObject mPausePage;
    private GameObject mBuildingStatusPage;
    private GameObject mRobotPage;
    private GameObject mMap;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        mBtnPause = FindUI("BtnPause").GetComponent<Button>();
        mBtnRobot = FindUI("BtnRobot").GetComponent<Button>();
        mBasePage = FindUI("Base").gameObject;
        mPausePage = FindUI("PausePage").gameObject;
        mRobotPage = FindUI("RobotPage").gameObject;
        mBuildingStatusPage = FindUI("BuildingStatus").gameObject;
        mMap = FindUI("Map").gameObject;
        
        mBtnPause.onClick.AddListener(OpenPausePage);
        mBtnRobot.onClick.AddListener(OnClickRobot);
    }
    
    Transform FindUI(string name)
    {
        return transform.FindChild(name);
    }

    public void OpenBasePage(BuildingBase b)
    {
        mBasePage.SetActive(true);
        mBasePage.GetComponent<UIBase>().Init(b);
        mMap.SetActive(false);
    }

    public void OpenPausePage()
    {
        mPausePage.SetActive(true);
        mMap.SetActive(false);
        GameManager.Instance.PauseGame();
    }

    public void ShowMap()
    {
        mMap.SetActive(true);
    }

    public void OpenBuildingStatusPage(BuildingBase b)
    {
        mBuildingStatusPage.SetActive(true);
        mBasePage.SetActive(false);
        mMap.SetActive(false);

        UIBuildingStatus uiStatus = mBuildingStatusPage.GetComponent<UIBuildingStatus>();
        uiStatus.InitView(b);
    }

    void OnClickRobot()
    {
        //Player.Instance.BuildRobot();
        mRobotPage.SetActive(true);
        mMap.SetActive(false);
    }
}
