using UnityEngine;
using System.Collections;

public class State_EnemyChase : StateMachineBehaviour {
    public int Frequency = 8;

    int _FrameCounter = 0;
    Transform _Owner;
    NavMeshAgent _Agent;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _Owner = animator.transform;
        _Agent = animator.GetComponent<NavMeshAgent>();
        float dis;
        Transform target = NearestPlayerRobots(out dis);
        if (target != null)
            _Agent.SetDestination(target.position);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _FrameCounter++;
        if(_FrameCounter >= Frequency)
        {
            _FrameCounter = 0;
            float minDis;
            Transform target = NearestPlayerRobots(out minDis);
            if(target == null)
            {
                animator.SetFloat("DistanceToPlayer", float.MaxValue);
            }
            else
            {
                _Agent.SetDestination(target.position);
                animator.SetFloat("DistanceToPlayer", minDis);
            }
        }
	}

    Transform NearestPlayerRobots(out float minDistance)
    {
        float minDis = float.MaxValue;
        Transform target = null;
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        foreach (RobotData r in player.RobotList)
        {
            float dis = Vector3.Distance(_Owner.position, r.transform.position);
            if (dis < minDis)
            {
                minDis = dis;
                target = r.transform;
            }
        }

        minDistance = minDis;
        return target;
    }
}
