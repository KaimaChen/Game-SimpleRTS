using UnityEngine;

public class Bullet : MonoBehaviour {
    public float LiveTime = 3; //存活时间（秒）

    private float mSpeed;
    private float mDemage;
    private float mTimer = 0;
    [SerializeField]
    private Transform mSource;
    [SerializeField]
    private Transform mTarget;
    
    void Update()
    {
        mTimer += Time.deltaTime;
        if(mTimer >= LiveTime)
        {
            Destroy(gameObject);
        }

        if(mTarget != null)
        {
            MoveToTarget();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if(mTarget != null && coll != null && coll.transform.gameObject == mTarget.gameObject)
        {
            if(coll.gameObject.CompareTag("Enemy") || coll.gameObject.CompareTag("Robot"))
            {
                RobotData data = coll.transform.GetComponent<RobotData>();
                data.DecreaseHp(mDemage);
            }
            else if(coll.gameObject.CompareTag("Building"))
            {
                Building b = coll.transform.GetComponent<Building>();
                b.BeingHit(mDemage);
            }
            Destroy(gameObject);
        }
        else
        {
            if (coll.gameObject != mSource.gameObject && !coll.gameObject.CompareTag("Bullet"))
                Destroy(gameObject);
        }
    }

    public void Spawn(Vector3 pos, Transform source, Transform target, float damage, float speed)
    {
        transform.position = pos;
        mSource = source;
        mTarget = target;
        mDemage = damage;
        mSpeed = speed;

        transform.rotation = Quaternion.LookRotation(target.position - transform.position);
    }

    void MoveToTarget()
    {
        Vector3 toTarget = mTarget.position - transform.position;
        toTarget.Normalize();

        transform.position += toTarget * mSpeed * Time.deltaTime;
    }
}
