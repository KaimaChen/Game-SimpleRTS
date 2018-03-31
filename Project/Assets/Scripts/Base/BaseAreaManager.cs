using UnityEngine;
using System.Collections.Generic;

public class BaseAreaManager : MonoBehaviour {
    public static BaseAreaManager Instance = null;

    private List<BaseArea> mBaseAreaList;
    public List<BaseArea> BaseAreaList
    {
        get { return mBaseAreaList; }
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

	void Start () {
        BaseArea[] areas = GetComponentsInChildren<BaseArea>();
        mBaseAreaList = new List<BaseArea>(areas);
	}

    public int SideAreaCount(Side side)
    {
        int count = 0;
        mBaseAreaList.ForEach((BaseArea area) =>
        {
            if (area.BelongSide == side)
                count++;
        });
        return count;
    }

    public int EnemyAreaCount()
    {
        return SideAreaCount(Side.Team2);
    }

    public int PlayerAreaCount()
    {
        return SideAreaCount(Side.Team1);
    }
}
