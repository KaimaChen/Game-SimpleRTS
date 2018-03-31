using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public static Player Instance = null;
    
    public Transform RobotSpawnPos;
    
    public List<BaseArea> BaseAreaList = new List<BaseArea>();
    
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
        RobotList.Add(r);
    }

    public void RemoveRobot(RobotData r)
    {
        RobotList.Remove(r);
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
    
    public GameObject CreateRobot(WeaponType weaponType, ChassisType chassisType)
    {
        GameObject weapon = Instantiate(ResourcesManager.Instance.GetPrefab(weaponType.ToString())) as GameObject;
        GameObject chassis = Instantiate(ResourcesManager.Instance.GetPrefab(chassisType.ToString())) as GameObject;
        GameObject robot = Instantiate(ResourcesManager.Instance.GetPrefab("RobotContainer"), RobotSpawnPos.position, Quaternion.identity, transform) as GameObject;
        chassis.name = "Chassis";
        chassis.transform.SetParent(robot.transform);
        chassis.transform.localRotation = Quaternion.identity;
        chassis.transform.localPosition = Vector3.zero;
        weapon.name = "Weapon";
        weapon.transform.SetParent(robot.transform);
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localPosition = Vector3.zero;
        robot.name = "Robot_" + chassisType.ToString() + "_" + weaponType.ToString();
        robot.transform.position = RobotSpawnPos.position;

        robot.GetComponent<RobotMotor>().chassisType = chassisType;
        robot.GetComponent<RobotAttack>().weaponType = weaponType;

        return robot;
    }
}
