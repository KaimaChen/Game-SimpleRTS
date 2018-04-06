using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RobotMotor))]
public class MineRobotAI : MonoBehaviour {
    public Transform Mineral;
    public int Frequency = 10; //多少帧做一次决策
    public int CollectMineralPerFrequency = 50; //每次决策收集多少矿石
    public float MinDistanceToOre = 3; //距离小于该值则视为到达金矿
    public float MinDistanceToBuilding = 5;
    public int UnloadMineralNeedFrequency = 5; //花多少决策时间来卸下金币到建筑物
    public int mMaxCarry = 500; //最大金币携带数量

    private int mFrameCounter = 0;
    private int mUnloadFrequencyCounter = 0;
    private bool mIsUnloading = false;
    private MineBuilding mBuildingOwner = null; //创建该机器人的建筑物，每次机器人拿到钱都是回到该建筑物
    [SerializeField]
    private int mCarriedGold = 0; //身上携带的金币数量
    
    private NavMeshAgent mAgent;
    private RobotMotor mMotor;
    private RobotData mData;
    private GoldOre mCurOre; //当前挖掘的金矿
    
    void Start()
    {
        mAgent = GetComponent<NavMeshAgent>();
        mMotor = GetComponent<RobotMotor>();
        mData = GetComponent<RobotData>();
        Mineral.localScale = Vector3.zero;
        mFrameCounter = Frequency;
    }

    void Update()
    {
        mFrameCounter++;
        if(mFrameCounter >= Frequency)
        {
            mFrameCounter = 0;
            UpdateMineral();
            
            if(mIsUnloading)
            {
                UpdateUnload();
            }
            else if(mCarriedGold < mMaxCarry)
            {
                UpdateCollectGold();
            }
            else
            {
                UpdateGobackToBuilding();
            }
        }
    }

    void OnApplicationPause(bool state)
    {
        if (state)
        {
            mAgent.Stop();
        }
        else
        {
            FindOreAndMove();
        }
    }

    public void Init(MineBuilding owner)
    {
        mBuildingOwner = owner;
    }

    void UpdateUnload()
    {
        mUnloadFrequencyCounter++;
        if (mUnloadFrequencyCounter >= UnloadMineralNeedFrequency)
        {
            mIsUnloading = false;
            mUnloadFrequencyCounter = 0;
            FindOreAndMove();
        }
    }

    void UpdateCollectGold()
    {
        if (mCurOre == null)
            FindOreAndMove();
        
        if (mCurOre != null && DistanceTo(mCurOre.transform) <= MinDistanceToOre)
        {
            mCarriedGold += mCurOre.GoldTaken(CollectMineralPerFrequency);

            Quaternion q = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mCurOre.transform.position - transform.position), Time.deltaTime * mData.RotateSpeed);
            transform.rotation = q;
        }
    }

    void UpdateGobackToBuilding()
    {
        if (DistanceTo(mBuildingOwner.transform) > MinDistanceToBuilding)
        {
            mMotor.MoveTo(mBuildingOwner.transform.position);
        }
        else if (mCarriedGold > 0)
        {
            mAgent.Stop();
            transform.rotation = Quaternion.LookRotation(mBuildingOwner.transform.position - transform.position);
            mBuildingOwner.CollectMoney(mCarriedGold);
            mCarriedGold = 0;
            mIsUnloading = true;
        }
    }
    
    /// <summary>
    /// 找到金矿并进行移动
    /// </summary>
    void FindOreAndMove()
    {
        mCurOre = NearestGoldOre();
        if(mCurOre != null)
        {
            mMotor.MoveTo(mCurOre.transform.position);
        }
    }

    /// <summary>
    /// 找到最近的金矿
    /// </summary>
    GoldOre NearestGoldOre()
    {
        GoldOreArea nearestArea = null;
        GoldOre nearestOre = null;
        float minDis = float.MaxValue;
        foreach(GoldOreArea area in GoldOreArea.GoldOreAreaList)
        {
            float dis = NavDistanceTo(area.NavTarget);
            if (minDis > dis)
            {
                nearestArea = area;
                minDis = dis;
            }
        }

        minDis = float.MaxValue;
        if(nearestArea != null)
        {
            foreach(GoldOre ore in nearestArea.GoldOres)
            {
                float dis = NavDistanceTo(ore.transform);
                if(minDis > dis)
                {
                    nearestOre = ore;
                    minDis = dis;
                }
            }
        }

        return nearestOre;
    }

    float NavDistanceTo(Transform target)
    {
        NavMeshPath path = new NavMeshPath();
        float dis = float.MaxValue;
        mAgent.CalculatePath(target.position, path);
        if (path.status != NavMeshPathStatus.PathPartial)
        {
            dis = PathLength(path);
        }

        return dis;
    }

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

    /// <summary>
    /// 计算x-z平面的距离
    /// </summary>
    float DistanceTo(Transform trans)
    {
        if (trans != null)
        {
            Vector2 p1 = new Vector2(trans.position.x, trans.position.z);
            Vector2 p2 = new Vector2(transform.position.x, transform.position.z);
            float dis = Vector2.Distance(p1, p2);
            return dis;
        }
        else
            return float.MaxValue;
    }

    void UpdateMineral()
    {
        float size = mCarriedGold / mMaxCarry;
        Mineral.localScale = new Vector3(size, size, size);
    }
}
