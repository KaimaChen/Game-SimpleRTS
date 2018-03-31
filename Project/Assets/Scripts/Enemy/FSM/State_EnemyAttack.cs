using UnityEngine;
using System.Collections;

public class State_EnemyAttack : StateMachineBehaviour {
    public int Frequency = 15;
    int _FrameCounter = 0;
    Transform _Owner;
    RobotAttack _RobotAttack;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _Owner = animator.transform;
        _RobotAttack = animator.GetComponent<RobotAttack>();
        float dis;
        _RobotAttack.LockTarget(NearestPlayerRobots(out dis));
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _FrameCounter++;
        if(_FrameCounter >= Frequency)
        {
            _FrameCounter = 0;
            float dis;
            _RobotAttack.LockTarget(NearestPlayerRobots(out dis));
            animator.SetFloat("DistanceToPlayer", dis);
        }
	}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _RobotAttack.Unlock();
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
