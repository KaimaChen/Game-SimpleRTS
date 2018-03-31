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
    private Side _Side = Side.Team1;
    public Side BelongSide
    {
        get { return _Side; }
        set
        {
            if (_Side != value)
                OnChangeSide(value);
            _Side = value;
        }
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
    }

    void OnChangeSide(Side newSide)
    {
        if (newSide == Side.Team1)
        {
            EnemyTeamAI.Instance.RobotList.Remove(this);
            Player.Instance.RobotList.Add(this);
        }
        else if (newSide == Side.Team2)
        {
            Player.Instance.RobotList.Remove(this);
            EnemyTeamAI.Instance.RobotList.Add(this);
        }
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
}
