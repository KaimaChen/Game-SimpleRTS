using UnityEngine;
using System.Collections.Generic;

public class EnemyTeamAI : MonoBehaviour {
    public static EnemyTeamAI Instance = null;

    public int Frequency = 10;
    public float Money = 100;
    public int GuardRobotNum = 1; //留多少机器人保护基地
    public int SearchRobotNum = 5; //使用多少机器人去开疆拓土
    public int AttackRobotNum = 3; //使用多少机器人去攻击玩家
    public List<RobotData> RobotList = new List<RobotData>();
    public List<BaseArea> BaseAreaList = new List<BaseArea>();
    
    private int mFrameCounter = 0;
    private WeaponType mFirstWeapon = WeaponType.Laser;
    private ChassisType mFirstChassis = ChassisType.Hover;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Update()
    {
        mFrameCounter++;
        if(mFrameCounter >= Frequency)
        {
            mFrameCounter = 0;

            if (CheckMineBuilding())
            {
                if (CheckRobotBuilding())
                {
                    CheckSeachRobot();
                }
            }
        }
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(new Vector2(0, 100), new Vector2(150, 80)), "Enemy Money: " + Money);
    }
    
    /// <summary>
    /// 没有造钱工厂，就要马上造一个（经济是最重要的）
    /// </summary>
    bool CheckMineBuilding()
    {
        if(HasBuilding(BuildingType.Mine) == false)
        {
            CreateBuilding(BuildingType.Mine);
            return false;
        }

        return true;
    }

    /// <summary>
    /// 检查建造机器人工厂
    /// </summary>
    /// <returns></returns>
    bool CheckRobotBuilding()
    {
        if(HasBuilding(BuildingType.RobotPart, mFirstWeapon) == false)
        {
            float price = GameConfig.Instance.GetBuildingPrice(BuildingType.RobotPart, mFirstWeapon);
            if(Money >= price)
            {
                Money -= price;
                return CreateBuilding(BuildingType.RobotPart, mFirstWeapon);
            }
        }

        if(HasBuilding(BuildingType.RobotPart, WeaponType.None, mFirstChassis) == false)
        {
            float price = GameConfig.Instance.GetBuildingPrice(BuildingType.RobotPart, WeaponType.None, mFirstChassis);
            if(Money >= price)
            {
                Money -= price;
                return CreateBuilding(BuildingType.RobotPart, WeaponType.None, mFirstChassis);
            }
        }

        return true;
    }
    
    /// <summary>
    /// 检查创建机器人去拓疆开土
    /// </summary>
    /// <returns></returns>
    bool CheckSeachRobot()
    {
        if(RobotList.Count < SearchRobotNum)
        {
            CreateRobot(mFirstWeapon, mFirstChassis);
            return false;
        }

        return true;
    }
    
    /// <summary>
    /// 找到空闲的建筑用地
    /// </summary>
    BuildingBase FindEmptyBase()
    {
        for(int i = 0; i < BaseAreaList.Count; i++)
        {
            BuildingBase build = BaseAreaList[i].FindEmptyBuildingBase();
            if (build != null)
                return build;
        }

        return null;
    }

    /// <summary>
    /// 建造对应类型的建筑
    /// </summary>
    /// <returns>是否成功开始建造</returns>
    bool CreateBuilding(BuildingType buildType, WeaponType weaponType = WeaponType.None, ChassisType chassisType = ChassisType.None)
    {
        BuildingBase emptyBase = FindEmptyBase();
        if (emptyBase == null || buildType == BuildingType.None)
            return false;

        float price = GameConfig.Instance.GetBuildingPrice(buildType, weaponType, chassisType);
        if (Money >= price)
        {
            emptyBase.Build(buildType, weaponType, chassisType);
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// 创建机器人
    /// </summary>
    bool CreateRobot(WeaponType weaponType, ChassisType chassisType)
    {
        float price = RobotData.GetRobotPrice(weaponType, chassisType);
        if(Money >= price)
        {
            Money -= price;
            RobotData.CreateEnemyRobot(weaponType, chassisType, transform, GetSpawnPos());
            return true;
        }

        return false;
    }

    /// <summary>
    /// 是否拥有指定的建筑类型
    /// </summary>
    bool HasBuilding(BuildingType buildType, WeaponType weaponType = WeaponType.None, ChassisType chassisType = ChassisType.None)
    {
        foreach(BaseArea area in BaseAreaList)
        {
            if (area.HasBuildingType(buildType, weaponType, chassisType))
                return true;
        }

        return false;
    }

    Vector3 GetSpawnPos()
    {
        return BaseAreaList[0].SpawnPos;
    }
}
