using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(RobotData))]
public class RobotAttack : MonoBehaviour {
    public WeaponType weaponType = WeaponType.Gun;
    
    private List<Transform> mSpawnPoses = new List<Transform>(); //子弹发射的位置
    private Transform mTarget;
    private List<LineRenderer> mLasers = new List<LineRenderer>();
    private Transform mWeapon;
    private RobotData mData = null;
    
    private float mTimer = 0;
    
    void Start()
    {
        mData = GetComponent<RobotData>();
        mData.InitWeapon(weaponType);

        SpawnPos[] spawnList = gameObject.GetComponentsInChildren<SpawnPos>();
        foreach(SpawnPos pos in spawnList)
        {
            mSpawnPoses.Add(pos.transform);
        }

        for(int i = 0; i < mSpawnPoses.Count; i++)
        {
            LineRenderer laser = Instantiate(ResourcesManager.Instance.GetPrefab("LaserLine")).GetComponent<LineRenderer>();
            laser.transform.SetParent(transform);
            mLasers.Add(laser);
        }

        mWeapon = transform.FindChild("Weapon");
        mTimer = CalAttackTimeDelta(); //保证一开始就能马上发一炮
    }
    
    void Update()
    {
        if(mTarget != null)
        {
            LookAt(mTarget);

            //看着敌人时才能发炮
            //if (!IsLookingAtTarget(mTarget))
            //    return;

            if (weaponType == WeaponType.Laser)
            {
                LaserAttack();
            }
            else
            {
                mTimer += Time.deltaTime;
                AbleLasers(false);
                if (mTimer >= CalAttackTimeDelta())
                {
                    mTimer = 0;
                    BulletAttack();
                }
            }
        }
        else
        {
            LookAt();
            AbleLasers(false);
        }
    }

    void OnDrawGizmos()
    {
        if (mData == null)
            return;
        
        Gizmos.color = new Color(1, 0, 0, 0.8f);
        Gizmos.DrawWireSphere(transform.position, mData.AttackRadius);
    }

    public void LockTarget(Transform target)
    {
        mTarget = target;
    }

    public void Unlock()
    {
        mTarget = null;
    }

	void BulletAttack()
    {
        float distance = Vector3.Distance(mTarget.position, transform.position);
        if (distance > mData.AttackRadius)
        {
            mTarget = null;
            return;
        }
        
        foreach(var spawn in mSpawnPoses)
        {
            GameObject bullet = Instantiate(ResourcesManager.Instance.GetPrefab(weaponType.ToString() + "Bullet")) as GameObject;
            bullet.GetComponent<Bullet>().Spawn(spawn.position, transform, mTarget, mData.Damage, mData.BulletSpeed);
        }
    }

    void LaserAttack()
    {
        float distance = Vector3.Distance(mTarget.position, transform.position);
        if (distance > mData.AttackRadius)
        {
            mTarget = null;
            AbleLasers(false);
            return;
        }

        AbleLasers(true);

        for(int i = 0; i < mLasers.Count; i++)
        {
            mLasers[i].SetPositions(new Vector3[] { mSpawnPoses[i].position, mTarget.position });
        }

        float damage = mData.Damage * Time.deltaTime * mLasers.Count;
        if (mTarget.CompareTag("Enemy")  || mTarget.CompareTag("Robot"))
            mTarget.GetComponent<RobotData>().DecreaseHp(damage);
        else if (mTarget.CompareTag("Building"))
            mTarget.GetComponent<Building>().BeingHit(damage);
    }

    /// <summary>
    /// 面向敌人
    /// </summary>
    /// <param name="target"></param>
    void LookAt(Transform target = null)
    {
        if(target != null)
        {
            Quaternion q = Quaternion.Lerp(mWeapon.rotation, Quaternion.LookRotation(target.position - mWeapon.position), Time.deltaTime * mData.RotateSpeed);
            mWeapon.rotation = q;
        }
        else
        {
            Quaternion q = Quaternion.Lerp(mWeapon.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * mData.RotateSpeed);
            mWeapon.localRotation = q;
        }
    }

    bool IsLookingAtTarget(Transform target)
    {
        Quaternion dir = Quaternion.LookRotation(target.position - mWeapon.position);
        return Quaternion.Angle(dir, mWeapon.rotation) < 0.1f;
    }

    float CalAttackTimeDelta()
    {
        float attackRate = mData.AttackRate;
        if (attackRate <= 0)
            return float.MaxValue;

        return 1.0f / attackRate; 
    }

    void AbleLasers(bool isEnable)
    {
        foreach (var laser in mLasers)
            laser.enabled = isEnable;
    }
}
