using UnityEngine;
using System.Collections.Generic;

public class SearchFSM_Search : StateMachineBehaviour {
    private Transform mTransform;
    private RobotMotor mMotor;
    private RobotAttack mAttack;
    private RobotData mData;
    private Enemy mSearchAI;
    private BaseArea mTargetArea = null;
    private Transform mTargetBuilding = null;
     
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        mTransform = animator.GetComponent<Transform>();
        mMotor = animator.GetComponent<RobotMotor>();
        mAttack = animator.GetComponent<RobotAttack>();
        mSearchAI = animator.GetComponent<Enemy>();
        mData = animator.GetComponent<RobotData>();
        
        MoveToNearestArea();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(mTargetArea != null)
        {
            mTargetArea.ProcessDoneEvent -= OnOccupyDone;
            mTargetArea.OtherTeamEnterAreaEvent -= OnEnterArea;
            mTargetBuilding = null;
            mTargetArea = null;
        }
    }

    void MoveToNearestArea()
    {
        BaseArea nearestArea = mSearchAI.FindNearestArea();
        if (nearestArea == null)
            return;

        mTargetArea = nearestArea;
        mTargetArea.ProcessDoneEvent += OnOccupyDone;
        mTargetArea.OtherTeamEnterAreaEvent += OnEnterArea;
        mMotor.MoveTo(mTargetArea.Center.position);
    }

    /// <summary>
    /// 占领完成
    /// </summary>
    void OnOccupyDone()
    {
        mTargetArea.ProcessDoneEvent -= OnOccupyDone;
        mTargetArea.OtherTeamEnterAreaEvent -= OnEnterArea;
        mTargetArea = null;

        MoveToNearestArea();
    }

    /// <summary>
    /// 进入该区域
    /// </summary>
    void OnEnterArea()
    {
        if (mTargetArea == null || mTargetBuilding != null)
            return;
        
        List<BuildingBase> bases = mTargetArea.OwnedBase;
        foreach(BuildingBase b in bases)
        {
            if(b.BuildingType != BuildingType.None)
            {
                mTargetBuilding = b.ContainedBuilding.transform;
                MoveToTargetInHalfAttackRadius(mTargetBuilding);
                mAttack.LockTarget(mTargetBuilding);
            }
        }
    }

    void MoveToTargetInHalfAttackRadius(Transform target)
    {
        Vector3 dir = mTransform.position - target.position;
        dir.Normalize();
        float halfAttackRaidus = mData.AttackRadius / 2;
        mMotor.MoveTo(target.position + dir * halfAttackRaidus);
    }
}
