using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public static Player Instance = null;
    
    public delegate void VoidDelegate();
    public event VoidDelegate MoneyChangeEvent;
    public event VoidDelegate BuildingChangeEvent;

    private List<BuildingBase> mBuildingList = new List<BuildingBase>();
    public List<BuildingBase> BuildingList
    {
        get { return mBuildingList; }
    }

    private List<RobotData> mRobotList = new List<RobotData>();
    public List<RobotData> RobotList
    {
        get { return mRobotList; }
    }

    [SerializeField]
    private float mMoney;
    public float Money
    {
        get { return mMoney; }

        set
        {
            mMoney = value;
            if (MoneyChangeEvent != null)
                MoneyChangeEvent.Invoke();
        }
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Money = GameConfig.ORIGIN_MONEY;
    }

    public void AddBuilding(BuildingBase build)
    {
        mBuildingList.Add(build);
        if (BuildingChangeEvent != null)
            BuildingChangeEvent.Invoke();
    }

    public void RemoveBuilding(BuildingBase build)
    {
        mBuildingList.Remove(build);
        if (BuildingChangeEvent != null)
            BuildingChangeEvent.Invoke();
    }
    
    public void AddRobot(RobotData r)
    {
        mRobotList.Add(r);
    }

    public void RemoveRobot(RobotData r)
    {
        mRobotList.Remove(r);
    }

    public List<RobotData> FindNeighbors(RobotData robot)
    {
        List<RobotData> neighbors = new List<RobotData>();
        foreach(RobotData r in RobotList)
        {
            Vector2 p1 = new Vector2(r.transform.position.x, r.transform.position.z);
            Vector2 p2 = new Vector2(robot.transform.position.x, robot.transform.position.z);
            float dis = Vector3.Distance(p1, p2);
            if (dis <= GameConfig.NEIGHBOR_DISTANCE)
            {
                neighbors.Add(r);
            }
        }
        return neighbors;
    }
}
