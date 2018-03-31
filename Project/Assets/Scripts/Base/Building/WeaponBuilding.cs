using UnityEngine;
using System.Collections.Generic;

public class WeaponBuilding : Building
{
    public float Frequency = 10; //每多少帧做一次决策
    public float RotateSpeed = 2;
    public Transform Weapon;
    public GameObject BulletPrefab;
    public float AttackRadius = 8; //攻击范围
    public float AttackAngle = 30; //左（右）多少度范围内才可以攻击
    public float AttackRate = 10;
    public float Damage = 10;
    public float BulletSpeed = 10;

    private List<Transform> SpawnPoses = new List<Transform>();
    private int mFrameCounter = 0;
    private float mAttackDeltaTime = 0;
    private float mTimer = 0;
    private Transform mTarget;
    
    protected override void Start()
    {
        base.Start();

        mOwner.Init(gameObject, BuildingType.Turret);
        mAttackDeltaTime = 1.0f / AttackRate;
        mTimer = mAttackDeltaTime;

        SpawnPos[] pos = GetComponentsInChildren<SpawnPos>();
        for(int i = 0; i < pos.Length; i++)
        {
            SpawnPoses.Add(pos[i].transform);
        }
    }

    void Update () {
        mFrameCounter++;
	    if(mFrameCounter >= Frequency)
        {
            mFrameCounter = 0;
            if (mTarget == null || Tools.Distance(mTarget.position, transform.position) > AttackRadius)
                FindNearEnemy();
        }

        LookAtTarget();
        Attack();
	}

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, AttackRadius);
    }

    void LookAtTarget()
    {
        if (mTarget == null)
            return;
        
        Quaternion rot = Quaternion.Lerp(Weapon.rotation, Quaternion.LookRotation(mTarget.position - Weapon.position), Time.deltaTime * RotateSpeed);
        Weapon.rotation = rot;
    }

    void Attack()
    {
        if (mTarget == null)
            return;

        Vector3 toTarget = mTarget.position - Weapon.position;
        float angle = Vector3.Angle(toTarget, Weapon.forward);
        mTimer += Time.deltaTime;
        if(angle <= AttackAngle && mTimer >= mAttackDeltaTime)
        {
            mTimer = 0;
            for(int i = 0; i < SpawnPoses.Count; i++)
            {
                GameObject go = Instantiate(BulletPrefab);
                Bullet bullet = go.GetComponent<Bullet>();
                bullet.Spawn(SpawnPoses[i].position, transform, mTarget, Damage, BulletSpeed);
            }
        }
    }

    void FindNearEnemy()
    {
        List<RobotData> enemys = null;
        if (BelongSide() == Side.Team1)
            enemys = EnemyTeamAI.Instance.RobotList;
        else if (BelongSide() == Side.Team2)
            enemys = Player.Instance.RobotList;

        float minDis = float.MaxValue;
        foreach(RobotData robot in enemys)
        {
            float dis = Tools.Distance(robot.transform.position, transform.position);
            if(dis < minDis)
            {
                minDis = dis;
                if(minDis <= AttackRadius)
                    mTarget = robot.transform;
            }
        }
    }
}
