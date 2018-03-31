using UnityEngine;
using System.Collections;

/// <summary>
/// 金矿
/// </summary>
public class GoldOre : MonoBehaviour {
    public const int MAX = 10000; //最大储量
    
    [SerializeField]
    private int mReserves = MAX; //储量
    private GoldOreArea mOwner = null;

    public GoldOreArea Owner
    {
        set { mOwner = value; }
    }

    public int GoldTaken(int num)
    {
        int goldsCanTaken = Mathf.Min(mReserves, num);

        mReserves -= num;
        if (mReserves <= 0)
        {
            if (mOwner != null)
                mOwner.Remove(this);
            Destroy(gameObject);
        }

        float percent = 1.0F * mReserves / MAX;
        float size = percent * 0.8f + 0.2f; //转到[0.2, 1]的区间，不取[0,0.2]是因为太小看不见
        transform.localScale = new Vector3(size, size, size);

        return goldsCanTaken;
    }
}
