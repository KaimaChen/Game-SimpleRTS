using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
    public float SenseRadius = 20; //感应范围
    public int Frequency = 10; //决策频率

    private int mFrameCounter;
    private NavMeshAgent mAgent;
    private RobotData mData;
    private RobotMotor mMotor;
    private RobotAttack mAttack;
    private Animator mAnima;

    private Transform mTarget = null;
    public Transform Target
    {
        get { return mTarget; }
    }

    void Start()
    {
        mAgent = GetComponent<NavMeshAgent>();
        mData = GetComponent<RobotData>();
        mMotor = GetComponent<RobotMotor>();
        mAttack = GetComponent<RobotAttack>();
        mAnima = GetComponent<Animator>();
        mFrameCounter = Frequency;
    }

    void Update()
    {
        mFrameCounter++;
        if (mFrameCounter >= Frequency)
        {
            mFrameCounter = 0;
            mAnima.SetFloat("Hp", mData.Hp);
            if (IsNearPlayer())
                mAnima.SetBool("Threaten", true);
            else
                mAnima.SetBool("Threaten", false);
        }
    }
    
    public BaseArea FindNearestArea()
    {
        List<BaseArea> otherAreaes = new List<BaseArea>();
        foreach(BaseArea area in BaseAreaManager.Instance.BaseAreaList)
        {
            if (area.BelongSide != Side.Team2)
                otherAreaes.Add(area);
        }

        float minDis = float.MaxValue;
        BaseArea baseArea = null;
        foreach(BaseArea area in otherAreaes)
        {
            float dis = DistanceToBaseArea(area);
            if (dis < minDis)
            {
                minDis = dis;
                baseArea = area;
            }
        }

        return baseArea;
    }

    float DistanceToBaseArea(BaseArea area)
    {
        NavMeshPath path = new NavMeshPath();
        mAgent.CalculatePath(area.Center.position, path);
        return PathLength(path);
    }

    /// <summary>
    /// 计算路径的长度
    /// </summary>
    float PathLength(NavMeshPath path)
    {
        if (path.corners.Length < 2)
            return 0;

        Vector3 previousCorner = path.corners[0];
        float lengthSoFar = 0.0F;
        int i = 1;
        while (i < path.corners.Length)
        {
            Vector3 currentCorner = path.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
            previousCorner = currentCorner;
            i++;
        }
        return lengthSoFar;
    }

    public bool IsNearPlayer()
    {
        var list = Player.Instance.RobotList;
        for (int i = 0; i < list.Count; i++)
        {
            if(Tools.Distance(transform.position, list[i].transform.position) <= SenseRadius)
            {
                mTarget = mTarget ?? list[i].transform;
                return true;
            }
        }

        mTarget = null;
        return false;
    }

    public void MoveAndAttackPlayer(Transform target)
    {
        mMotor.MoveTo(target.position);
        if(Tools.Distance(transform.position, target.position) < mData.AttackRadius / 2)
        {
            mMotor.Stop();
            mAttack.LockTarget(target);
        }
    }
}
