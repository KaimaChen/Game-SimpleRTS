using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIRobotPage : MonoBehaviour {
    const string NONE = "<color=#ff0000>None</color>";
    const string READY = "<color=#00ff00>Ready</color>";

    Toggle mToggleGun;
    Toggle mToggleLaser;
    Toggle mToggleCannon;
    Toggle mToggleRocket;
    Toggle mToggleWheels;
    Toggle mToggleHover;
    Toggle mToggleLegs;
    Toggle mToggleTracks;
    Text mGunDesc;
    Text mLaserDesc;
    Text mCannonDesc;
    Text mRocketDesc;
    Text mWheelsDesc;
    Text mHoverDesc;
    Text mLegsDesc;
    Text mTracksDesc;

    Text mDamage;
    Text mArmor;
    Text mPrice;
    Button mBtnBuy;

    float mTotalPrice;
    WeaponType mSelectedWeaponType;
    ChassisType mSelectedChassisType;
    Dictionary<string, bool> mPartsReady = new Dictionary<string, bool>();
    
	void Start () {
        mToggleGun = FindToggle("Weapons/Gun/Toggle");
        mToggleGun.onValueChanged.AddListener(OnChangeGun);
        mToggleLaser = FindToggle("Weapons/Laser/Toggle");
        mToggleLaser.onValueChanged.AddListener(OnChangeLaser);
        mToggleCannon = FindToggle("Weapons/Cannon/Toggle");
        mToggleCannon.onValueChanged.AddListener(OnChangeCannon);
        mToggleRocket = FindToggle("Weapons/Rocket/Toggle");
        mToggleRocket.onValueChanged.AddListener(OnChangeRocket);
        mToggleWheels = FindToggle("Chassis/Wheels/Toggle");
        mToggleWheels.onValueChanged.AddListener(OnChangeWheels);
        mToggleHover = FindToggle("Chassis/Hover/Toggle");
        mToggleHover.onValueChanged.AddListener(OnChangeHover);
        mToggleLegs = FindToggle("Chassis/Legs/Toggle");
        mToggleLegs.onValueChanged.AddListener(OnChangeLegs);
        mToggleTracks = FindToggle("Chassis/Tracks/Toggle");
        mToggleTracks.onValueChanged.AddListener(OnChangeTracks);

        mGunDesc = FindText("Weapons/Gun/Desc");
        mLaserDesc = FindText("Weapons/Laser/Desc");
        mCannonDesc = FindText("Weapons/Cannon/Desc");
        mRocketDesc = FindText("Weapons/Rocket/Desc");
        mWheelsDesc = FindText("Chassis/Wheels/Desc");
        mHoverDesc = FindText("Chassis/Hover/Desc");
        mLegsDesc = FindText("Chassis/Legs/Desc");
        mTracksDesc = FindText("Chassis/Tracks/Desc");

        mDamage = FindText("Properties/Damage");
        mArmor = FindText("Properties/Armor");
        mPrice = FindText("Price");
        mBtnBuy = transform.FindChild("BtnBuy").GetComponent<Button>();
        mBtnBuy.onClick.AddListener(
            () =>
            {
                Player.Instance.Money -= mTotalPrice;
                RobotData.CreateRobot(mSelectedWeaponType, mSelectedChassisType, Player.Instance.transform, Vector3.zero);
                CloseView();
            });

        transform.FindChild("BtnClose").GetComponent<Button>().onClick.AddListener(CloseView);

        mPartsReady.Add(WeaponType.Gun.ToString(), false);
        mPartsReady.Add(WeaponType.Laser.ToString(), false);
        mPartsReady.Add(WeaponType.Cannon.ToString(), false);
        mPartsReady.Add(WeaponType.Rocket.ToString(), false);
        mPartsReady.Add(ChassisType.Wheels.ToString(), false);
        mPartsReady.Add(ChassisType.Hover.ToString(), false);
        mPartsReady.Add(ChassisType.Legs.ToString(), false);
        mPartsReady.Add(ChassisType.Tracks.ToString(), false);
        mPartsReady.Add("None", false);

        mSelectedWeaponType = WeaponType.Gun;
        mSelectedChassisType = ChassisType.Wheels;

        Player.Instance.BuildingChangeEvent += CheckRobotPartsState;
        CheckRobotPartsState();
    }

    void CloseView()
    {
        gameObject.SetActive(false);
        UIManager.Instance.ShowMap();
    }
	
    Toggle FindToggle(string path)
    {
        return transform.FindChild(path).GetComponent<Toggle>();
    }

    Text FindText(string path)
    {
        return transform.FindChild(path).GetComponent<Text>();
    }

    void ResetPartsReady()
    {
        mPartsReady[WeaponType.Gun.ToString()] = false;
        mPartsReady[WeaponType.Laser.ToString()] = false;
        mPartsReady[WeaponType.Cannon.ToString()] = false;
        mPartsReady[WeaponType.Rocket.ToString()] = false;
        mPartsReady[ChassisType.Wheels.ToString()] = false;
        mPartsReady[ChassisType.Hover.ToString()] = false;
        mPartsReady[ChassisType.Legs.ToString()] = false;
        mPartsReady[ChassisType.Tracks.ToString()] = false;
        mPartsReady["None"] = false;
    }

    void CheckRobotPartsState()
    {
        ResetPartsReady();

        foreach (BuildingBase building in Player.Instance.BuildingList)
        {
            if (building.BuildingType != BuildingType.RobotPart)
                continue;

            mPartsReady[building.WeaponType.ToString()] = true;
            mPartsReady[building.ChassisType.ToString()] = true;
        }

        mGunDesc.text = mPartsReady[WeaponType.Gun.ToString()] ? READY : NONE;
        mLaserDesc.text = mPartsReady[WeaponType.Laser.ToString()] ? READY : NONE;
        mCannonDesc.text = mPartsReady[WeaponType.Cannon.ToString()] ? READY : NONE;
        mRocketDesc.text = mPartsReady[WeaponType.Rocket.ToString()] ? READY : NONE;
        mWheelsDesc.text = mPartsReady[ChassisType.Wheels.ToString()] ? READY : NONE;
        mHoverDesc.text = mPartsReady[ChassisType.Hover.ToString()] ? READY : NONE;
        mLegsDesc.text = mPartsReady[ChassisType.Legs.ToString()] ? READY : NONE;
        mTracksDesc.text = mPartsReady[ChassisType.Tracks.ToString()] ? READY : NONE;

        RobotShowControler.Instance.ShowWeapon(mSelectedWeaponType, mPartsReady[mSelectedWeaponType.ToString()]);
        RobotShowControler.Instance.ShowChassis(mSelectedChassisType, mPartsReady[mSelectedChassisType.ToString()]);
        SetData();
    }

    void SetData()
    {
        GameConfig config = GameConfig.Instance;
        mTotalPrice = config.GetWeaponPrice(mSelectedWeaponType);
        mTotalPrice += config.GetChassisPrice(mSelectedChassisType);
        mPrice.text = "$ " + mTotalPrice.ToString();

        mDamage.text = "伤害: " + config.GetDamage(mSelectedWeaponType).ToString();
        mArmor.text = "护甲: " + config.GetArmor(mSelectedChassisType).ToString();

        mBtnBuy.interactable = Player.Instance.Money >= mTotalPrice && mPartsReady[mSelectedWeaponType.ToString()] && mPartsReady[mSelectedChassisType.ToString()];
    }

    void OnChangeWeapon(WeaponType type, bool isSelected)
    {
        if(isSelected)
        {
            RobotShowControler.Instance.ShowWeapon(type, mPartsReady[type.ToString()]);
            mSelectedWeaponType = type;
            SetData();
        }
    }

    void OnChangeChassis(ChassisType type, bool isSelected)
    {
        if(isSelected)
        {
            RobotShowControler.Instance.ShowChassis(type, mPartsReady[type.ToString()]);
            mSelectedChassisType = type;
            SetData();
        }
    }

    void OnChangeGun(bool isSelected)
    {
        OnChangeWeapon(WeaponType.Gun, isSelected);
    }

    void OnChangeLaser(bool isSelected)
    {
        OnChangeWeapon(WeaponType.Laser, isSelected);
    }

    void OnChangeCannon(bool isSelected)
    {
        OnChangeWeapon(WeaponType.Cannon, isSelected);
    }

    void OnChangeRocket(bool isSelected)
    {
        OnChangeWeapon(WeaponType.Rocket, isSelected);
    }

    void OnChangeWheels(bool isSelected)
    {
        OnChangeChassis(ChassisType.Wheels, isSelected);
    }

    void OnChangeHover(bool isSelected)
    {
        OnChangeChassis(ChassisType.Hover, isSelected);
    }

    void OnChangeLegs(bool isSelected)
    {
        OnChangeChassis(ChassisType.Legs, isSelected);
    }

    void OnChangeTracks(bool isSelected)
    {
        OnChangeChassis(ChassisType.Tracks, isSelected);
    }
}
