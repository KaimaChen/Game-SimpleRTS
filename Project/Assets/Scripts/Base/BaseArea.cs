using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 基地区域是属于某一方的，可以被占领
/// 有建筑或敌人时不能占领
/// </summary>
public class BaseArea : MonoBehaviour {
    public Transform Center;
    public int Frequency = 10;
    public Side BelongSide = Side.Team1;
    public float SenseRadius = 10;

    public delegate void VoidEventHandler();
    public VoidEventHandler ProcessDoneEvent; //占领结束事件
    public VoidEventHandler OtherTeamEnterAreaEvent; //别的队进入到区域的检测范围
    
    private bool mFirstFrame = true;
    private int mFrameCounter = 0;
    private RoundProcess mProcess;
    private Side mTargetSide;

    private List<BuildingBase> mOwnedBase;
    public List<BuildingBase> OwnedBase
    {
        get { return mOwnedBase; }
    }

    private Vector3 mSpawnPos;
    public Vector3 SpawnPos
    {
        get { return mSpawnPos; }
    }
    
    void Awake()
    {
        mProcess = transform.Find("RoundProcess").GetComponent<RoundProcess>();
    }

	void Start () {
        BuildingBase[] bases = GetComponentsInChildren<BuildingBase>();
        mOwnedBase = new List<BuildingBase>(bases);

        ResetProcess();
        mProcess.FillEvent += OnProcessDone;
        
        SetBelongSide(BelongSide);
        mSpawnPos = GetComponentInChildren<SpawnPos>().transform.position;
    }
	
	void Update () {
        if(mFirstFrame)
        {
            mFirstFrame = false;
            Init();
            return;
        }

        mFrameCounter++;
        if(mFrameCounter >= Frequency)
        {
            mFrameCounter = 0;
            if(IsSenseOtherTeam() && !IsSenseOurTeam() && HasBuilding() == false)
            {
                if(mProcess.IsFilling == false)
                    StartProcess();
            }
            else
            {
                ResetProcess();
            }
        }
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(Center.position, SenseRadius);
    }
    
    void Init()
    {
        SetBelongSide(BelongSide);
        foreach (BuildingBase b in mOwnedBase)
        {
            b.BelongSide = BelongSide;
        }
    }

    RobotData IsSenseTeam(Side side)
    {
        List<RobotData> robots = new List<RobotData>();
        if (side == Side.Team1)
            robots = Player.Instance.RobotList;
        else if (side == Side.Team2)
            robots = EnemyTeamAI.Instance.RobotList;
        else
            robots = RobotData.RobotList;

        foreach (RobotData robot in robots)
        {
            if (Tools.Distance(robot.transform.position, Center.position) <= SenseRadius)
            {
                return robot;
            }
        }

        return null;
    }
    
    bool IsSenseOtherTeam()
    {
        Side side = Side.Team2;
        if (BelongSide == Side.Team2)
            side = Side.Team1;
        else if (BelongSide == Side.None)
            side = Side.None;

        RobotData robot = IsSenseTeam(side);
        if (robot != null)
            mTargetSide = robot.BelongSide;
        bool result = robot != null;
        if(result && OtherTeamEnterAreaEvent != null)
        {
            OtherTeamEnterAreaEvent.Invoke();
        }

        return result;
    }

    bool IsSenseOurTeam()
    {
        if (BelongSide == Side.None)
            return false;
        else
            return IsSenseTeam(BelongSide) != null;
    }

    void ResetProcess()
    {
        mProcess.Reset();
        mProcess.gameObject.SetActive(false);
    }

    void StartProcess()
    {
        mProcess.IsFilling = true;
        mProcess.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 占领进度完成事件
    /// </summary>
    void OnProcessDone()
    {
        mProcess.Reset();

        SetBelongSide(mTargetSide);

        if (ProcessDoneEvent != null)
            ProcessDoneEvent.Invoke();
    }

    /// <summary>
    /// 该区域是否有建筑物
    /// </summary>
    bool HasBuilding()
    {
        foreach (BuildingBase b in mOwnedBase)
        {
            if (b.BuildingType != BuildingType.None)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 是否有指定的建筑类型
    /// </summary>
    public bool HasBuildingType(BuildingType buildType, WeaponType weaponType = WeaponType.None, ChassisType chassisType = ChassisType.None)
    {
        foreach(BuildingBase b in mOwnedBase)
        {
            if (b.IsTypeOf(buildType, weaponType, chassisType))
                return true;
        }

        return false;
    }

    void SetBelongSide(Side side)
    {
        if (BelongSide != side)
            OnChangSide(side);

        BelongSide = side;

        if(side == Side.Team1)
        {
            EnemyTeamAI.Instance.BaseAreaList.Remove(this);
        }
        else if(side == Side.Team2)
        {
            EnemyTeamAI.Instance.BaseAreaList.Add(this);
        }
    }

    void OnChangSide(Side newSide)
    {
        foreach(BuildingBase b in OwnedBase)
        {
            b.BelongSide = newSide;
        }
    }

    public BuildingBase FindEmptyBuildingBase()
    {
        foreach (BuildingBase b in OwnedBase)
        {
            if (b.BuildingType == BuildingType.None)
                return b;
        }

        return null;
    }
}
