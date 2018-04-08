using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(RobotData))]
public class RobotMotor : MonoBehaviour {
    public ChassisType chassisType = ChassisType.Wheels;

    private NavMeshAgent mAgent;
    private RobotData mData;
    private bool mIsFirstFrame = true;

	void Start () {
        mAgent = this.GetComponent<NavMeshAgent>();
        mAgent.Warp(transform.position);

        mData = GetComponent<RobotData>();
        mData.InitChassis(chassisType);
	}

    void Update()
    {
        if(mIsFirstFrame)
        {
            InitData();
            mIsFirstFrame = false;
        }
    }

    void InitData()
    {
        mAgent.speed = mData.MoveSpeed;
    }
	
    public void MoveTo(Vector3 target)
    {
        if (mAgent == null)
            mAgent = GetComponent<NavMeshAgent>();
        mAgent.Resume();
        mAgent.SetDestination(target);
    }

    public void Stop()
    {
        mAgent.Stop();
    }
}
