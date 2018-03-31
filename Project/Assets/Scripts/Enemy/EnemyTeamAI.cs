using UnityEngine;
using System.Collections.Generic;

public class EnemyTeamAI : MonoBehaviour {
    public static EnemyTeamAI Instance = null;

    public int Frequency = 10;
    public float Money = 100;
    public int GuardRobotNum = 1; //留多少机器人保护基地
    public int SearchRobotNum = 1; //使用多少机器人去开疆拓土
    public int AttackRobotNum = 3; //使用多少机器人去攻击玩家
    public Transform SpawnPos;
    public BaseArea MainBaseArea;
    public List<RobotData> RobotList = new List<RobotData>();
    public List<BaseArea> BaseAreaList = new List<BaseArea>();

    private int _FrameCounter = 0;
    
    #region Unity
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Update()
    {
        _FrameCounter++;
        if(_FrameCounter >= Frequency)
        {
            _FrameCounter = 0;
            
            if(CheckMainBaseArea())
            {
                if(CheckMoneyBuilding())
                {
                    if(CheckRobotBuilding())
                    {
                        if(CheckGuardRobot())
                        {
                            if(CheckSeachRobot())
                            {
                                CheckAttackRobot();
                            }
                        }
                    }
                }
            }
        }
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(new Vector2(0, 100), new Vector2(100, 80)), "Money: " + Money);
    }
    #endregion

    //TODO
    bool CheckMainBaseArea()
    {
        if (MainBaseArea.BelongSide != Side.Team2)
            return false;

        return true;
    }

    /// <summary>
    /// 没有造钱工厂，就要马上造一个（经济是最重要的）
    /// </summary>
    bool CheckMoneyBuilding()
    {
        if(MainBaseArea.HasBuildingType(BuildingType.Mine) == false)
        {
            Build(BuildingType.Mine);
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
        //if(RobotBuildings.Count <= 0)
        //{
        //    Build(BuildingType.RobotPart);
        //    return false;
        //}

        return true;
    }

    /// <summary>
    /// 查看是否有保护基地的机器人
    /// </summary>
    bool CheckGuardRobot()
    {
        if(RobotList.Count >= GuardRobotNum)
        {
            for(int i = 0; i < GuardRobotNum; i++)
            {
                //TODO
                //RobotList[0].GetComponent<EnemyAI>().CreateWayPointsIn(MainBaseArea);
            }
            
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 检查创建机器人去拓疆开土
    /// </summary>
    /// <returns></returns>
    bool CheckSeachRobot()
    {
        if(RobotList.Count < GuardRobotNum + SearchRobotNum)
        {
            CreateRobot();
            return false;
        }

        return true;
    }

    /// <summary>
    /// 检查创建攻击机器人
    /// </summary>
    /// <returns></returns>
    bool CheckAttackRobot()
    {
        int count = GuardRobotNum + SearchRobotNum + AttackRobotNum;
        if (RobotList.Count < count)
        {
            CreateRobot();
            return false;
        }
        else
        {
            int startIndex = GuardRobotNum + SearchRobotNum;
            for (int i = startIndex; i < count; i++)
            {
                //Test
                RobotList[i].GetComponent<RobotMotor>().MoveTo(new Vector3(0.59f, -3.5f, 26.53711f));
            }
            return true;
        }
    }

    /// <summary>
    /// 找到空闲的建筑用地
    /// </summary>
    BuildingBase FindEmptyBase(BaseArea area)
    {
        List<BuildingBase> bases = area.OwnedBase;
        foreach(BuildingBase b in bases)
        {
            if (b.BuildingType == BuildingType.None)
                return b;
        }

        return null;
    }

    /// <summary>
    /// 建造对应类型的建筑
    /// </summary>
    /// <returns>是否成功开始建造</returns>
    bool Build(BuildingType type)
    {
        BuildingBase emptyBase = FindEmptyBase(MainBaseArea);
        if (emptyBase == null || type == BuildingType.None)
            return false;

        //TODO
        //if (Money >= GameConfig.Instance.PriceList[(int)type])
        //{
        //   emptyBase.BuildingNow(type);
        //    return true;
        //}

        return false;
    }

    /// <summary>
    /// 创建机器人
    /// </summary>
    bool CreateRobot()
    {
        //if(RobotBuildings.Count > 0 && Money >= GameConfig.Instance.RobotCost)
        //{
        //    Money -= GameConfig.Instance.RobotCost;
        //    RobotBuildings[0].RobotPrefab = ResourcesManager.Instance.GetPrefab("Enemy");
        //    RobotBuildings[0].CreateRobot(SpawnPos.position, Side.Team2, transform);
        //    return true;
        //}

        return false;
    }

    /// <summary>
    /// 是否拥有指定的建筑类型
    /// </summary>
    bool HasBuilding(BuildingType type)
    {
        foreach(BaseArea area in BaseAreaList)
        {
            if (area.HasBuildingType(type))
                return true;
        }

        return false;
    }
}
