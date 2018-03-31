using UnityEngine;
using UnityEngine.UI;

public class UIBase : MonoBehaviour {
    public Button BtnClose;
    public GameObject Map;

    public Toggle ToggleSupport;
    public Toggle ToggleChassis;
    public Toggle ToggleWeapon;

    public GameObject SupportPage;
    public GameObject ChassisPage;
    public GameObject WeaponPage;

    public Button BtnMine;
    public Button BtnTurret;
    public Button BtnWheels;
    public Button BtnHover;
    public Button BtnLegs;
    public Button BtnTracks;
    public Button BtnGun;
    public Button BtnLaser;
    public Button BtnCannon;
    public Button BtnRocket;

    private BuildingBase mBase;
    private float mMinePrice;
    private float mTurretPrice;
    private float mWheelsPrice;
    private float mHoverPrice;
    private float mLegsPrice;
    private float mTracksPrice;
    private float mGunPrice;
    private float mLaserPrice;
    private float mCannonPrice;
    private float mRocketPrice;

    void Start()
    {
        BtnClose.onClick.AddListener(OnClickCloseButton);

        ToggleSupport.onValueChanged.AddListener(OnToggleSupport);
        ToggleChassis.onValueChanged.AddListener(OnToggleChassis);
        ToggleWeapon.onValueChanged.AddListener(OnToggleWeapon);

        BtnMine.onClick.AddListener(OnClickMine);
        BtnTurret.onClick.AddListener(OnClickTurret);
        BtnWheels.onClick.AddListener(OnClickWheels);
        BtnHover.onClick.AddListener(OnClickHover);
        BtnLegs.onClick.AddListener(OnClickLegs);
        BtnTracks.onClick.AddListener(OnClickTracks);
        BtnGun.onClick.AddListener(OnClickGun);
        BtnLaser.onClick.AddListener(OnClickLaser);
        BtnRocket.onClick.AddListener(OnClickRocket);

        Player.Instance.MoneyChangeEvent += CheckEnoughMoney;

        InitPrice();
        InitUI();
        CheckEnoughMoney();
    }
    
    public void Init(BuildingBase b)
    {
        mBase = b;
    }
    
    void InitPrice()
    {
        GameConfig config = GameConfig.Instance;
        mMinePrice = config.GetBuildingPrice(BuildingType.Mine);
        mTurretPrice = config.GetBuildingPrice(BuildingType.Turret);
        mWheelsPrice = config.GetBuildingPrice(BuildingType.RobotPart, WeaponType.None, ChassisType.Wheels);
        mHoverPrice = config.GetBuildingPrice(BuildingType.RobotPart, WeaponType.None, ChassisType.Hover);
        mLegsPrice = config.GetBuildingPrice(BuildingType.RobotPart, WeaponType.None, ChassisType.Legs);
        mTracksPrice = config.GetBuildingPrice(BuildingType.RobotPart, WeaponType.None, ChassisType.Tracks);
        mGunPrice = config.GetBuildingPrice(BuildingType.RobotPart, WeaponType.Gun);
        mLaserPrice = config.GetBuildingPrice(BuildingType.RobotPart, WeaponType.Laser);
        mCannonPrice = config.GetBuildingPrice(BuildingType.RobotPart, WeaponType.Cannon);
        mRocketPrice = config.GetBuildingPrice(BuildingType.RobotPart, WeaponType.Rocket);
    }

    void InitUI()
    {
        SetPrice(BtnMine, mMinePrice);
        SetPrice(BtnTurret, mTurretPrice);
        SetPrice(BtnWheels, mWheelsPrice);
        SetPrice(BtnHover, mHoverPrice);
        SetPrice(BtnLegs, mLegsPrice);
        SetPrice(BtnTracks, mTracksPrice);
        SetPrice(BtnGun, mGunPrice);
        SetPrice(BtnLaser, mLaserPrice);
        SetPrice(BtnCannon, mCannonPrice);
        SetPrice(BtnRocket, mRocketPrice);

        SetPropertyDamage(BtnGun, WeaponType.Gun);
        SetPropertyDamage(BtnLaser, WeaponType.Laser);
        SetPropertyDamage(BtnCannon, WeaponType.Cannon);
        SetPropertyDamage(BtnRocket, WeaponType.Rocket);

        SetPropertyArmor(BtnWheels, ChassisType.Wheels);
        SetPropertyArmor(BtnHover, ChassisType.Hover);
        SetPropertyArmor(BtnLegs, ChassisType.Legs);
        SetPropertyArmor(BtnTracks, ChassisType.Tracks);

        SupportPage.SetActive(true);
        WeaponPage.SetActive(false);
        ChassisPage.SetActive(false);
    }
    
    void SetPrice(Button btn, float price)
    {
        btn.transform.FindChild("Price").GetComponent<Text>().text = "$ " + price;
    }

    void SetPropertyDamage(Button btn, WeaponType weaponType)
    {
        btn.transform.FindChild("Properties").GetComponent<Text>().text = "伤害:" + GameConfig.Instance.GetDamage(weaponType);
    }

    void SetPropertyArmor(Button btn, ChassisType chassisType)
    {
        btn.transform.FindChild("Properties").GetComponent<Text>().text = "护甲:" + GameConfig.Instance.GetArmor(chassisType);
    }

    void OnClickCloseButton()
    {
        InputManager.Instance.IsOpen = true;
        Map.SetActive(true);
        gameObject.SetActive(false);
    }

    void OnToggleSupport(bool isSelected)
    {
        SupportPage.SetActive(isSelected);
    }

    void OnToggleChassis(bool isSelected)
    {
        ChassisPage.SetActive(isSelected);
    }

    void OnToggleWeapon(bool isSelected)
    {
        WeaponPage.SetActive(isSelected);
    }

    void OnClickMine()
    {
        mBase.Build(BuildingType.Mine);
        OnClickCloseButton();
    }

    void OnClickTurret()
    {
        mBase.Build(BuildingType.Turret);
        OnClickCloseButton();
    }

    void OnClickWheels()
    {
        mBase.Build(BuildingType.RobotPart, WeaponType.None, ChassisType.Wheels);
        OnClickCloseButton();
    }

    void OnClickHover()
    {
        mBase.Build(BuildingType.RobotPart, WeaponType.None, ChassisType.Hover);
        OnClickCloseButton();
    }

    void OnClickLegs()
    {
        mBase.Build(BuildingType.RobotPart, WeaponType.None, ChassisType.Legs);
        OnClickCloseButton();
    }

    void OnClickTracks()
    {
        mBase.Build(BuildingType.RobotPart, WeaponType.None, ChassisType.Tracks);
        OnClickCloseButton();
    }

    void OnClickGun()
    {
        mBase.Build(BuildingType.RobotPart, WeaponType.Gun);
        OnClickCloseButton();
    }

    void OnClickLaser()
    {
        mBase.Build(BuildingType.RobotPart, WeaponType.Laser);
        OnClickCloseButton();
    }

    void OnClickCannon()
    {
        mBase.Build(BuildingType.RobotPart, WeaponType.Cannon);
        OnClickCloseButton();
    }

    void OnClickRocket()
    {
        mBase.Build(BuildingType.RobotPart, WeaponType.Rocket);
        OnClickCloseButton();
    }

    void CheckEnoughMoney()
    {
        float money = Player.Instance.Money;
        BtnMine.interactable = money >= mMinePrice;
        BtnTurret.interactable = money >= mTurretPrice;
        BtnWheels.interactable = money >= mWheelsPrice;
        BtnHover.interactable = money >= mHoverPrice;
        BtnLegs.interactable = money >= mLegsPrice;
        BtnTracks.interactable = money >= mTracksPrice;
        BtnGun.interactable = money >= mGunPrice;
        BtnLaser.interactable = money >= mLaserPrice;
        BtnCannon.interactable = money >= mCannonPrice;
        BtnRocket.interactable = money >= mRocketPrice;
    }
}
