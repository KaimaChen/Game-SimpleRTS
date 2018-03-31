using UnityEngine;
using System.Collections.Generic;

public class GameConfig : MonoBehaviour {
    public static GameConfig Instance = null;

    public const float NEIGHBOR_DISTANCE = 5; //双击选中附近的机器人的范围
    public const int ORIGIN_MONEY = 100; //初始金币数
    public const int BUILDING_SPEED = 50; //建筑物建造速度
    public const int BUILDING_MAX_HP = 100; //建筑物的最大血量
    public const float ENEMY_SELECTE_TIME = 2.0f; //敌人选中状态的持续时间

    public Transform LeftUp;
    public Transform RightBottom;

    public int RobotCost = 50;
    
    public Dictionary<string, float> WeaponData = new Dictionary<string, float>();
    public Dictionary<string, float> ChassisData = new Dictionary<string, float>();
    public Dictionary<string, float> BuildingData = new Dictionary<string, float>();

    private Vector2 mSceneHorizontal;
    public Vector2 SceneHorizontal
    {
        get { return mSceneHorizontal; }
    }

    private Vector2 mSceneVertical;
    public Vector2 SceneVertical
    {
        get { return mSceneVertical; }
    }

    private float mSceneWidth;
    public float SceneWidth
    {
        get { return (mSceneHorizontal.y - mSceneHorizontal.x); }
    }

    private float mSceneHeight;
    public float SceneHeight
    {
        get { return (mSceneVertical.y - mSceneVertical.x); }
    }

    void Awake()
    {
        if (Instance == null)
        {
            InitData();
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        mSceneHorizontal = new Vector2(LeftUp.position.x, RightBottom.position.x);
        mSceneVertical = new Vector2(RightBottom.position.z, LeftUp.position.z);
    }
    
    void InitData()
    {
        InitWeaponData();
        InitChassisData();
        InitBuildingData();
    }

    void InitWeaponData()
    {
        WeaponData.Add("GunAttackRate", 5);
        WeaponData.Add("GunAttackRadius", 10);
        WeaponData.Add("GunDamage", 5);
        WeaponData.Add("GunSpeed", 60);
        WeaponData.Add("GunPrice", 50);
        WeaponData.Add("GunBuildingPrice", 50);

        WeaponData.Add("CannonAttackRate", 3);
        WeaponData.Add("CannonAttackRadius", 12);
        WeaponData.Add("CannonDamage", 9);
        WeaponData.Add("CannonSpeed", 30);
        WeaponData.Add("CannonPrice", 100);
        WeaponData.Add("CannonBuildingPrice", 100);

        WeaponData.Add("LaserAttackRate", 0);
        WeaponData.Add("LaserAttackRadius", 14);
        WeaponData.Add("LaserDamage", 27);
        WeaponData.Add("LaserSpeed", 0);
        WeaponData.Add("LaserPrice", 150);
        WeaponData.Add("LaserBuildingPrice", 150);

        WeaponData.Add("RocketAttackRate", 1);
        WeaponData.Add("RocketAttackRadius", 18);
        WeaponData.Add("RocketDamage", 28);
        WeaponData.Add("RocketSpeed", 25);
        WeaponData.Add("RocketPrice", 200);
        WeaponData.Add("RocketBuildingPrice", 200);
    }

    void InitChassisData()
    {
        ChassisData.Add("WheelsSpeed", 10);
        ChassisData.Add("WheelsArmor", 2);
        ChassisData.Add("WheelsPrice", 50);
        ChassisData.Add("WheelsBuildingPrice", 50);

        ChassisData.Add("HoverSpeed", 11);
        ChassisData.Add("HoverArmor", 3);
        ChassisData.Add("HoverPrice", 100);
        ChassisData.Add("HoverBuildingPrice", 100);

        ChassisData.Add("LegsSpeed", 12);
        ChassisData.Add("LegsArmor", 5);
        ChassisData.Add("LegsPrice", 150);
        ChassisData.Add("LegsBuildingPrice", 150);

        ChassisData.Add("TracksSpeed", 13);
        ChassisData.Add("TracksArmor", 7);
        ChassisData.Add("TracksPrice", 200);
        ChassisData.Add("TracksBuildingPrice", 200);
    }

    void InitBuildingData()
    {
        BuildingData.Add("MinePrice", 100);
        BuildingData.Add("TurretPrice", 100);
    }

    public float GetAttackRate(WeaponType type)
    {
        return WeaponData[type.ToString() + "AttackRate"];
    }

    public float GetAttackRadius(WeaponType type)
    {
        return WeaponData[type.ToString() + "AttackRadius"];
    }

    public float GetDamage(WeaponType type)
    {
        return WeaponData[type.ToString() + "Damage"];
    }

    public float GetBulletSpeed(WeaponType type)
    {
        return WeaponData[type.ToString() + "Speed"];
    }

    public float GetWeaponPrice(WeaponType type)
    {
        return WeaponData[type.ToString() + "Price"];
    }

    public float GetArmor(ChassisType type)
    {
        return ChassisData[type.ToString() + "Armor"];
    }

    public float GetMoveSpeed(ChassisType type)
    {
        return ChassisData[type.ToString() + "Speed"];
    }

    public float GetChassisPrice(ChassisType type)
    {
        return ChassisData[type.ToString() + "Price"];
    }

    public float GetBuildingPrice(BuildingType buildType, WeaponType weaponType = WeaponType.None, ChassisType chassisType = ChassisType.None)
    {
        if(buildType == BuildingType.RobotPart)
        {
            if (weaponType != WeaponType.None)
                return WeaponData[weaponType.ToString() + "BuildingPrice"];
            else if (chassisType != ChassisType.None)
                return ChassisData[chassisType.ToString() + "BuildingPrice"];
            else
                return 0;
        }
        else if(buildType == BuildingType.None)
        {
            return 0;
        }
        else
        {
            return BuildingData[buildType.ToString() + "Price"];
        }
    }
}
