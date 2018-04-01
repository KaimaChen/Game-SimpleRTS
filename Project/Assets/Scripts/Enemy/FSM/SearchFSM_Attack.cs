using UnityEngine;
using System.Collections;

public class SearchFSM_Attack : StateMachineBehaviour {
    private RobotAttack mAttack;
    private SearchEnemyAI mSearch;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mAttack = animator.GetComponent<RobotAttack>();
        mSearch = animator.GetComponent<SearchEnemyAI>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(mSearch.Target != null)
        {
            mAttack.LockTarget(mSearch.Target);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
