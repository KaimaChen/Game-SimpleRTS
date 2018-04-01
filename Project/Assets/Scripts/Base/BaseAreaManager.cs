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

    int SideAreaCount(Side side)
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

    Vector3 GetSpawnPos(Side side)
    {
        Vector3 pos = Vector3.zero;
        for(int i = 0; i < mBaseAreaList.Count; i++)
        {
            if(mBaseAreaList[i].BelongSide == side)
            {
                pos = mBaseAreaList[i].SpawnPos;
                break;
            }
        }

        return pos;
    }

    public Vector3 GetPlayerSpawnPos()
    {
        return GetSpawnPos(Side.Team1);
    }

    public Vector3 GetEnemySpawnPos()
    {
        return GetSpawnPos(Side.Team2);
    }
    
}
