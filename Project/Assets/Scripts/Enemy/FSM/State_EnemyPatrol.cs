using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 敌人巡逻状态
/// </summary>
public class State_EnemyPatrol : StateMachineBehaviour {
    public int Frequency = 10; //10帧更新一次

    private int _FrameCounter = 0;
    private int _CurrentWaypoint = 0;
    private float _DisToChangeWayPoint = 2.1f; //距离小于多少时更改路点
    private Transform _Owner;
    private NavMeshAgent _Agent;
    private EnemyAI _EnemyAI;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _Owner = animator.transform;
        _Agent = _Owner.GetComponent<NavMeshAgent>();
        _EnemyAI = _Owner.GetComponent<EnemyAI>();
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _FrameCounter++;
        if(_FrameCounter >= Frequency)
        {
            _FrameCounter = 0;
            if(_EnemyAI.WayPoints.Count > 0)
            {
                float dis = Vector3.Distance(_Owner.transform.position, _EnemyAI.WayPoints[_CurrentWaypoint]);
                if (dis <= _DisToChangeWayPoint)
                    ChangeWayPoint();
                _Agent.SetDestination(_EnemyAI.WayPoints[_CurrentWaypoint]);
            }
            
            animator.SetFloat("DistanceToPlayer", MinDistanceToPlayerRobots());
        }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
    
    void ChangeWayPoint()
    {
        _CurrentWaypoint++;
        if (_CurrentWaypoint >= _EnemyAI.WayPoints.Count)
            _CurrentWaypoint = 0;
    }

    float MinDistanceToPlayerRobots()
    {
        float minDis = float.MaxValue;
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        foreach(RobotData r in player.RobotList)
        {
            float dis = Vector3.Distance(_Owner.position, r.transform.position);
            if (dis < minDis)
                minDis = dis;
        }

        return minDis;
    }
}
