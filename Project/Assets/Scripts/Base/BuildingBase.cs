using UnityEngine;

public class BuildingBase : MonoBehaviour {
    public Side BelongSide = Side.Team2;
    
    public delegate void VoidDelegate();
    public event VoidDelegate ChangeHpEvent;
    public event VoidDelegate BuildDoneEvent;

    private BuildingType mBuildingType = BuildingType.None;
    private WeaponType mWeaponType = WeaponType.None;
    private ChassisType mChassisType = ChassisType.None;
    private Transform mBuildingContainter;
    private Transform mHp;
    private Transform mHpBorder;
    private GameObject mBuilding = null;
    private float mTimeToBuildOneHp = float.MaxValue;
    private float mTimer = 0;

    private string mName;
    public string Name
    {
        get { return mName; }
    }

    private bool mIsBuildingNow = false; //是否正在建筑中
    public bool IsBuildingNow
    {
        get { return mIsBuildingNow; }
    }

    private float mCurrentHp;
    public float CurrentHp
    {
        get { return mCurrentHp; }
        set
        {
            mCurrentHp = value;
            if(ChangeHpEvent != null)
                ChangeHpEvent();
        }
    }

    public GameObject ContainedBuilding
    {
        get { return mBuilding; }
    }

    public BuildingType BuildingType
    {
        get { return mBuildingType; }
    }

    public WeaponType WeaponType
    {
        get { return mWeaponType; }
    }

    public ChassisType ChassisType
    {
        get { return mChassisType; }
    }

    void Awake()
    {
        mBuildingContainter = transform.FindChild("BuildingContainer");
        mHp = transform.FindChild("Hp");
        mHpBorder = transform.FindChild("HpBorder");
    }

	void Start () {
        InputManager.Instance.LeftClickDownEvent += JudgeSelected;

        mTimeToBuildOneHp = 1.0f / GameConfig.Instance.BuildingSpeed;
        ChangeHpEvent += UpdateHpBar;

        if (mBuildingType == BuildingType.None) //Init方法是在Start前调用
        {
            mHp.gameObject.SetActive(false);
            mHpBorder.gameObject.SetActive(false);
        }
    }
	
	void Update () {
	    if(mIsBuildingNow)
        {
            mTimer += Time.deltaTime;
            if (mTimer >= mTimeToBuildOneHp)
            {
                float times = mTimer / mTimeToBuildOneHp;
                CurrentHp += times;
                mTimer -= times * mTimeToBuildOneHp;
            }
                
            if (CurrentHp >= GameConfig.Instance.BuildingMaxHp)
            {
                mCurrentHp = GameConfig.Instance.BuildingMaxHp;
                BuildComplete();
            }
        }
	}

    public void Init(GameObject building, BuildingType buildType, WeaponType weaponType = WeaponType.None, ChassisType chassisType = ChassisType.None)
    {
        mBuilding = building;
        mCurrentHp = GameConfig.Instance.BuildingMaxHp;
        SetType(buildType, weaponType, chassisType);
        mHp.gameObject.SetActive(true);
        mHpBorder.gameObject.SetActive(true);
    }

    public void SetType(BuildingType buildType, WeaponType weaponType = WeaponType.None, ChassisType chassisType = ChassisType.None)
    {
        mBuildingType = buildType;
        mWeaponType = weaponType;
        mChassisType = chassisType;
        Player.Instance.AddBuilding(this);
    }

    public bool IsTypeOf(BuildingType buildType, WeaponType weaponType = WeaponType.None, ChassisType chassisType = ChassisType.None)
    {
        if(buildType != BuildingType.RobotPart)
        {
            return mBuildingType == buildType;
        }
        else
        {
            return mBuildingType == buildType && mWeaponType == weaponType && mChassisType == chassisType;
        }
    }

    /// <summary>
    /// 查看建筑物的详情或进行建筑
    /// </summary>
    public void OpenBuildingMsg()
    {
        InputManager.Instance.IsOpen = false;
        if (mBuildingType != BuildingType.None)
            UIManager.Instance.OpenBuildingStatusPage(this);
        else
            UIManager.Instance.OpenBasePage(this);
    }
    
    public void Build(BuildingType buildType, WeaponType weaponType = WeaponType.None, ChassisType chassisType = ChassisType.None)
    {
        if (buildType == BuildingType.None)
            return;

        mBuildingType = buildType;
        mWeaponType = weaponType;
        mChassisType = chassisType;
        mName = GetBuildingName(buildType, weaponType, chassisType);
        mIsBuildingNow = true;
        mCurrentHp = 0;

        float price = GameConfig.Instance.GetBuildingPrice(buildType, weaponType, chassisType);
        if (BelongSide == Side.Team1)
            Player.Instance.Money -= price;
        else if (BelongSide == Side.Team2)
            EnemyTeamAI.Instance.Money -= price;

        GameObject notCompleteBuilding = Instantiate(ResourcesManager.Instance.GetPrefab("NotCompleteBuilding")) as GameObject;
        notCompleteBuilding.transform.SetParent(mBuildingContainter);
        notCompleteBuilding.transform.localPosition = Vector3.zero;
        notCompleteBuilding.name = "Building";
        mBuilding = notCompleteBuilding;
    }
    
    /// <summary>
    /// 减少HP
    /// </summary>
    public void DecreaseHp(float num)
    {
        CurrentHp -= num;
        if(CurrentHp <= 0 && mBuilding != null)
        {
            DestroyBuilding();
        }
    }
    
    public void SellBuilding()
    {
        if(mBuildingType != BuildingType.None)
        {
            Player.Instance.Money += GameConfig.Instance.GetBuildingPrice(mBuildingType, mWeaponType, mChassisType) / 2;
            DestroyBuilding();
            mIsBuildingNow = false;
        }
    }

    public string HpText()
    {
        return mCurrentHp + "/" + GameConfig.Instance.BuildingMaxHp;
    }

    void DestroyBuilding()
    {
        mBuildingType = BuildingType.None;
        mWeaponType = WeaponType.None;
        mChassisType = ChassisType.None;

        if(BelongSide == Side.Team1)
            Player.Instance.RemoveBuilding(this);

        if (mBuilding != null)
        {
            Destroy(mBuilding);
            mBuilding = null;
        }
        mHp.gameObject.SetActive(false);
        mHpBorder.gameObject.SetActive(false);
    }

    string GetBuildingName(BuildingType buildType, WeaponType weaponType = WeaponType.None, ChassisType chassisType = ChassisType.None)
    {
        if (buildType == BuildingType.RobotPart)
        {
            if (weaponType != WeaponType.None)
                return weaponType.ToString();
            else
                return chassisType.ToString();
        }
        else
        {
            return buildType.ToString();
        }
    }

    void BuildComplete()
    {
        mIsBuildingNow = false;

        GameObject notComplete = mBuildingContainter.FindChild("Building").gameObject;
        if (notComplete != null)
            Destroy(notComplete);

        mBuilding = CreateBuilding();
        mBuilding.transform.SetParent(mBuildingContainter);
        mBuilding.transform.localPosition = Vector3.zero;
        mBuilding.name = "Building";

        if(BelongSide == Side.Team1)
            Player.Instance.AddBuilding(this);

        if (BuildDoneEvent != null)
            BuildDoneEvent.Invoke();
    }
    
    GameObject CreateBuilding()
    {
        GameObject building = null;
        if (mBuildingType == BuildingType.RobotPart)
        {
            if (mWeaponType != WeaponType.None)
            {
                string name = mBuildingType.ToString() + "_" + mWeaponType.ToString();
                building = Instantiate(ResourcesManager.Instance.GetPrefab(name)) as GameObject;
            }
            else if (mChassisType != ChassisType.None)
            {
                string name = mBuildingType.ToString() + "_" + mChassisType.ToString();
                building = Instantiate(ResourcesManager.Instance.GetPrefab(name)) as GameObject;
            }
        }
        else if (mBuildingType != BuildingType.None)
        {
            building = Instantiate(ResourcesManager.Instance.GetPrefab(mBuildingType.ToString())) as GameObject;
        }

        return building;
    }

    void UpdateHpBar()
    {
        if (mBuildingType == BuildingType.None)
        {
            mHp.gameObject.SetActive(false);
            mHpBorder.gameObject.SetActive(false);
        }
        else
        {
            mHpBorder.gameObject.SetActive(true);
            mHp.gameObject.SetActive(true);
            mHp.localScale = new Vector3(1, 1, HpPercent());
        }
    }

    float HpPercent()
    {
        float percent = 1.0f * CurrentHp / GameConfig.Instance.BuildingMaxHp;
        percent = Mathf.Clamp(percent, 0, 1);
        return percent;
    }

    void JudgeSelected()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider.gameObject == gameObject || hitInfo.collider.gameObject == mBuilding)
            {
                if (BelongSide == Side.Team1) //自己的建筑才能查看
                {
                    OpenBuildingMsg();
                }
            }
        }
    }
}
