using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RobotInteraction))]
[RequireComponent(typeof(RobotMotor))]
[RequireComponent(typeof(RobotAttack))]
[RequireComponent(typeof(RobotData))]

public class RobotInput : MonoBehaviour {
    public GameObject ClickSign;

    private RobotInteraction mInteraction;
    private RobotMotor mMotor;
    private RobotAttack mAttack;
    private RobotData mRobot;
    private float mLastLeftClick = 0; //上回左键点击的时间（用于判断双击）
    private float mDoubleClickGap = 0.5f;
    private bool mIsLastTimeSelected = false; //上次是否是选中的

	void Start () {
        mInteraction = this.GetComponent<RobotInteraction>();
        mMotor = this.GetComponent<RobotMotor>();
        mAttack = this.GetComponent<RobotAttack>();
        mRobot = this.GetComponent<RobotData>();
        
        InputManager.Instance.LeftClickUpEvent += MouseClick;
	}

    void OnDestroy()
    {
        InputManager.Instance.LeftClickUpEvent -= MouseClick;
    }
	
    void MouseClick()
    {
        bool isLastTimeSelected = mIsLastTimeSelected;
        mIsLastTimeSelected = false;

        RaycastHit hitInfo;
        if (IsClickSomething(out hitInfo))
        {
            GameObject hitGo = hitInfo.collider.gameObject;
            if (hitGo == gameObject && hitGo.tag == "Robot") //选中自己
            {
                mIsLastTimeSelected = true;
                mInteraction.Select();

                if(isLastTimeSelected && (Time.time - mLastLeftClick <= mDoubleClickGap)) //双击选中附近的同伴
                {
                    StartCoroutine(SelectAllNeighbors());
                }
            }
            else if(hitGo.tag == "Enemy") //选中敌人
            {
                if(IsSelectThisPlayer())
                {
                    RobotInteraction enemyInteraction = hitGo.GetComponent<RobotInteraction>();
                    enemyInteraction.Select(GameConfig.ENEMY_SELECTE_TIME);
                    mAttack.LockTarget(hitGo.transform);
                }
            }
            else if (hitGo.layer == LayerMask.NameToLayer("Ground") || hitGo.layer == LayerMask.NameToLayer("Sea")) //点到地表
            {
                if (IsSelectThisPlayer())
                {
                    GameObject clickSign = Instantiate(ClickSign) as GameObject;
                    clickSign.transform.position = hitInfo.point + Vector3.up * 0.01f;

                    mMotor.MoveTo(hitInfo.point);
                }
            }
            else if(hitGo.tag == "Building")
            {
                Building b = hitGo.GetComponent<Building>();
                if(IsSelectThisPlayer() && b.BelongSide() != mRobot.BelongSide)
                {
                    mAttack.LockTarget(b.transform);
                }
            }
            else
            {
                mInteraction.Deselect();
            }
        }
        mLastLeftClick = Time.time;
    }
    
    bool IsClickSomething(out RaycastHit hitInfo)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo))
        {
            return true;
        }
        
        return false;
    }

    IEnumerator SelectAllNeighbors()
    {
        yield return new WaitForEndOfFrame();
        List<RobotData> neighbors = Player.Instance.FindNeighbors(mRobot);
        foreach (RobotData r in neighbors)
        {
            RobotInteraction interaction = r.GetComponent<RobotInteraction>();
            interaction.Select();
        }
    }

    bool IsSelectThisPlayer()
    {
        return mInteraction.IsSelected && (gameObject.tag == "Robot");
    }
}
