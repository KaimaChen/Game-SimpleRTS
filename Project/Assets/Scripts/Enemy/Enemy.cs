using UnityEngine;
using System.Collections;

/// <summary>
/// 自动攻击附近的玩家
/// </summary>
public class Enemy : MonoBehaviour {
    public int CheckPlayerFrequency = 10; 
    public int CheckBuildingFrequency = 5;

    private int _CheckPlayerCounter = 0;
    private int _CheckBuildingCounter = 0;
    private RobotAttack _Attack;
    private RobotData mData;
    private RobotData _AttackTarget = null;
    
	void Start () {
        _Attack = GetComponent<RobotAttack>();
        mData = GetComponent<RobotData>();
    }
	
	void Update () {
        CheckPlayer();
        CheckBuilding();
	}

    void CheckPlayer()
    {
        _CheckPlayerCounter++;
        if(_CheckPlayerCounter >= CheckPlayerFrequency)
        {
            _CheckPlayerCounter = 0;
            RobotData robot = NearestPlayerRobot();
            float distance = Vector3.Distance(robot.transform.position, transform.position);
            if(robot != null && distance <= mData.AttackRadius)
            {
                _AttackTarget = robot;
                _Attack.LockTarget(_AttackTarget.transform);
            }
            else
            {
                _AttackTarget = null;
            }
        }
    }

    void CheckBuilding()
    {
        _CheckBuildingCounter++;
        if(_CheckBuildingCounter >= CheckBuildingFrequency)
        {
            _CheckBuildingCounter = 0;
            if (_AttackTarget != null) //优先攻击机器人
                return;

            //TODO
        }
    }

    RobotData NearestPlayerRobot()
    {
        float minDis = float.MaxValue;
        RobotData nearestRobot = null;
        foreach(RobotData robot in Player.Instance.RobotList)
        {
            float distance = Vector3.Distance(robot.transform.position, transform.position);
            if(distance < minDis)
            {
                minDis = distance;
                nearestRobot = robot;
            }
        }

        return nearestRobot;
    }

    //TODO
    /*
    Building NearestPlayerBuilding()
    {
        float minDis = float.MaxValue;
        Building nearestBuilding = null;
        BaseArea nearestArea = NearestPlayerArea();
        if(nearestArea != null)
        {
            foreach(Base b in nearestArea.OwnedBase)
            {
                if (b.OwnBuildingType == BuildingType.None)
                    continue;

                float dis = Vector3.Distance(b.transform.position, transform.position);
                if(dis < minDis)
                {
                    minDis = dis;
                    //nearestBuilding = b.GetComponent<Building>();
                }
            }
        }
        return nearestBuilding;
    }
    */
    BaseArea NearestPlayerArea()
    {
        float minDis = float.MaxValue;
        BaseArea nearestArea = null;
        foreach(BaseArea area in BaseAreaManager.Instance.BaseAreaList)
        {
            if (area.BelongSide == Side.Team2)
                continue;

            float dis = Vector3.Distance(transform.position, area.Center.position);
            if(dis < minDis)
            {
                minDis = dis;
                nearestArea = area;
            }
        }

        return nearestArea;
    }
}
