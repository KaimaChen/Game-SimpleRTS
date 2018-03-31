using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class State_Search : StateMachineBehaviour {
    private Transform _Transform;
    private RobotMotor _Motor;
    private RobotAttack _Attack;
    private RobotData mData;
    private SearchEnemyAI _SearchAI;
    private BaseArea _TargetArea = null;
    private Transform _TargetBuilding = null;
     
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _Transform = animator.GetComponent<Transform>();
        _Motor = animator.GetComponent<RobotMotor>();
        _Attack = animator.GetComponent<RobotAttack>();
        _SearchAI = animator.GetComponent<SearchEnemyAI>();
        mData = animator.GetComponent<RobotData>();
        
        MoveToNearestArea();
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
    
    bool MoveToNearestArea()
    {
        BaseArea nearestArea = _SearchAI.FindNearestArea();
        if (nearestArea == null)
            return false;

        _TargetArea = nearestArea;
        _TargetArea.ProcessDoneEvent += OnOccupyDone;
        _TargetArea.OtherTeamEnterAreaEvent += OnEnterArea;
        _Motor.MoveTo(nearestArea.Center.position);
        return true;
    }

    /// <summary>
    /// 占领完成
    /// </summary>
    void OnOccupyDone()
    {
        _TargetArea.ProcessDoneEvent -= OnOccupyDone;
        _TargetArea.OtherTeamEnterAreaEvent -= OnEnterArea;
        _TargetArea = null;

        MoveToNearestArea();
    }

    /// <summary>
    /// 进入该区域
    /// </summary>
    void OnEnterArea()
    {
        if (_TargetArea == null || _TargetBuilding != null)
            return;
        
        List<BuildingBase> bases = _TargetArea.OwnedBase;
        foreach(BuildingBase b in bases)
        {
            if(b.BuildingType != BuildingType.None)
            {
                _TargetBuilding = b.ContainedBuilding.transform;
                MoveToTargetInHalfAttackRadius(_TargetBuilding);
                _Attack.LockTarget(_TargetBuilding);
            }
        }
    }

    void MoveToTargetInHalfAttackRadius(Transform target)
    {
        Vector3 dir = _Transform.position - target.position;
        dir.Normalize();
        float halfAttackRaidus = mData.AttackRadius / 2;
        _Motor.MoveTo(target.position + dir * halfAttackRaidus);
    }
}
