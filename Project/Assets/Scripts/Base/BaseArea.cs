using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 基地区域是属于某一方的，可以被占领
/// </summary>
public class BaseArea : MonoBehaviour {
    public Transform Center;
    public int Frequency = 10;
    public Side BelongSide = Side.Team1;
    public float SenseRadius = 10;

    public delegate void VoidEventHandler();
    public VoidEventHandler ProcessDoneEvent; //占领结束事件
    public VoidEventHandler OtherTeamEnterAreaEvent; //别的队进入到区域的检测范围
    
    private bool _FirstFrame = true;
    private int _FrameCounter = 0;
    private RoundProcess _Process;
    private List<BuildingBase> mOwnedBase;

    public List<BuildingBase> OwnedBase
    {
        get { return mOwnedBase; }
    }

    #region unity
    void Awake()
    {
        _Process = transform.Find("RoundProcess").GetComponent<RoundProcess>();
    }

	void Start () {
        BuildingBase[] bases = GetComponentsInChildren<BuildingBase>();
        mOwnedBase = new List<BuildingBase>(bases);

        ResetProcess();
        _Process.FillEvent += OnProcessDone;
        
        OnChangSide(BelongSide);
    }
	
	void Update () {
        if(_FirstFrame)
        {
            _FirstFrame = false;
            Init();
            return;
        }

        _FrameCounter++;
        if(_FrameCounter >= Frequency)
        {
            _FrameCounter = 0;
            if(IsSenseOtherTeam() && HasBuilding() == false)
            {
                if(_Process.IsFilling == false)
                {
                    StartProcess();
                }
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
    
    #endregion

    void Init()
    {
        SetBelongSide(BelongSide);
        foreach (BuildingBase b in mOwnedBase)
        {
            b.BelongSide = BelongSide;
        }

        if (BelongSide == Side.Team1)
            Player.Instance.BaseAreaList.Add(this);
        else if (BelongSide == Side.Team2)
            EnemyTeamAI.Instance.BaseAreaList.Add(this);
    }

    /// <summary>
    /// 是否检测到其他队伍进入
    /// </summary>
    bool IsSenseOtherTeam()
    {
        List<RobotData> robots = new List<RobotData>();
        if (BelongSide == Side.Team1)
            robots = EnemyTeamAI.Instance.RobotList;
        else if (BelongSide == Side.Team2)
            robots = Player.Instance.RobotList;

        foreach(RobotData robot in robots)
        {
            if (Tools.Distance(robot.transform.position, Center.position) <= SenseRadius)
            {
                if (OtherTeamEnterAreaEvent != null)
                    OtherTeamEnterAreaEvent.Invoke();
                return true;
            }
        }
        
        return false;
    }

    void ResetProcess()
    {
        _Process.Reset();
        _Process.gameObject.SetActive(false);
    }

    void StartProcess()
    {
        _Process.IsFilling = true;
        _Process.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 占领进度完成事件
    /// </summary>
    void OnProcessDone()
    {
        _Process.Reset();

        if (BelongSide == Side.Team1)
            SetBelongSide(Side.Team2);
        else if (BelongSide == Side.Team2)
            SetBelongSide(Side.Team1);

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
    public bool HasBuildingType(BuildingType type)
    {
        foreach(BuildingBase b in mOwnedBase)
        {
            if (b.BuildingType == type)
                return true;
        }

        return false;
    }

    void SetBelongSide(Side side)
    {
        if (BelongSide != side)
            OnChangSide(side);

        BelongSide = side;
    }

    void OnChangSide(Side newSide)
    {
        if(newSide == Side.Team1)
        {
            Player.Instance.BaseAreaList.Add(this);
            EnemyTeamAI.Instance.BaseAreaList.Remove(this);
        }
        else if(newSide == Side.Team2)
        {
            EnemyTeamAI.Instance.BaseAreaList.Add(this);
            Player.Instance.BaseAreaList.Remove(this);
        }

        foreach(BuildingBase b in OwnedBase)
        {
            b.BelongSide = newSide;
        }
    }
}
