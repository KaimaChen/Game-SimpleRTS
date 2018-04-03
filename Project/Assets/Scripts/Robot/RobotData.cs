using UnityEngine;
using System.Collections;

public class RobotData : MonoBehaviour {
    public GameObject MapPointPrefab;
    public float Hp = 100;
    public float MaxHp = 100;

    private Transform mMapPoint;

    [SerializeField]
    private float mMoveSpeed = 10;
    public float MoveSpeed
    {
        get { return mMoveSpeed; }
    }

    [SerializeField]
    private float mRotateSpeed = 20;
    public float RotateSpeed
    {
        get { return mRotateSpeed; }
    }

    [SerializeField]
    private float mArmor = 0;
    public float Armor
    {
        get { return mArmor; }
    }

    [SerializeField]
    private float mAttackRate; //攻击速度（次数/秒）
    public  float AttackRate
    {
        get { return mAttackRate; }
    }

    [SerializeField]
    private float mAttackRadius;
    public float AttackRadius
    {
        get { return mAttackRadius; }
    }

    [SerializeField]
    private float mDamage;
    public float Damage
    {
        get { return mDamage; }
    }

    [SerializeField]
    private float mBulletSpeed;
    public float BulletSpeed
    {
        get { return mBulletSpeed; }
    }

    [SerializeField]
    private Side mSide = Side.Team1;
    public Side BelongSide
    {
        get { return mSide; }
    }

    void Start()
    {
        mMapPoint = (Instantiate(MapPointPrefab) as GameObject).transform;
        mMapPoint.SetParent(MapManager.Instance.transform);

        if (BelongSide == Side.Team1)
            Player.Instance.AddRobot(this);
        else if (BelongSide == Side.Team2)
            EnemyTeamAI.Instance.RobotList.Add(this);
    }

    void Update()
    {
        mMapPoint.localPosition = MapManager.Instance.WorldPos2MapPos(transform.position);

        if(Hp <= 0)
        {
            Player.Instance.RemoveRobot(this);
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (BelongSide == Side.Team1)
            Player.Instance.RemoveRobot(this);
        else if (BelongSide == Side.Team2)
            EnemyTeamAI.Instance.RobotList.Remove(this);

        Destroy(mMapPoint.gameObject);
    }
    
    public void InitChassis(ChassisType type)
    {
        mArmor = GameConfig.Instance.GetArmor(type);
        mMoveSpeed = GameConfig.Instance.GetMoveSpeed(type);
    }

    public void InitWeapon(WeaponType type)
    {
        mAttackRate = GameConfig.Instance.GetAttackRate(type);
        mAttackRadius = GameConfig.Instance.GetAttackRadius(type);
        mDamage = GameConfig.Instance.GetDamage(type);
        mBulletSpeed = GameConfig.Instance.GetBulletSpeed(type);
    }

    public float HpPercent()
    {
        return 1.0f * Hp / MaxHp;
    }

    public void DecreaseHp(float demage)
    {
        Hp -= demage * ((10 - mArmor) / 10);
        Hp = Hp < 0 ? 0 : Hp;
    }

    static GameObject CreateRawRobot(WeaponType weaponType, ChassisType chassisType, Transform parent, string prefabName)
    {
        Vector3 spawnPos = BaseAreaManager.Instance.GetPlayerSpawnPos();
        GameObject weapon = Instantiate(ResourcesManager.Instance.GetPrefab(weaponType.ToString())) as GameObject;
        GameObject chassis = Instantiate(ResourcesManager.Instance.GetPrefab(chassisType.ToString())) as GameObject;
        GameObject robot = Instantiate(ResourcesManager.Instance.GetPrefab(prefabName), spawnPos, Quaternion.identity, parent) as GameObject;
        chassis.name = "Chassis";
        chassis.transform.SetParent(robot.transform);
        chassis.transform.localRotation = Quaternion.identity;
        chassis.transform.localPosition = new Vector3(0, -0.55f, 0);
        weapon.name = "Weapon";
        weapon.transform.SetParent(robot.transform);
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localPosition = Vector3.zero;
        robot.name = "Robot_" + chassisType.ToString() + "_" + weaponType.ToString();

        robot.GetComponent<RobotMotor>().chassisType = chassisType;
        robot.GetComponent<RobotAttack>().weaponType = weaponType;

        return robot;
    }

    public static GameObject CreateRobot(WeaponType weaponType, ChassisType chassisType, Transform parent)
    {
        return CreateRawRobot(weaponType, chassisType, parent, "RobotContainer");
    }

    public static GameObject CreateSearchRobot(WeaponType weaponType, ChassisType chassisType, Transform parent)
    {
        return CreateRawRobot(weaponType, chassisType, parent, "SearchRobotContainer");
    }
}
