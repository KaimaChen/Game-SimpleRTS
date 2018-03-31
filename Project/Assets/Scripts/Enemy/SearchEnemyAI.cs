using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 搜索型AI
/// 负责独自去占领区域的
/// </summary>
public class SearchEnemyAI : MonoBehaviour {
    private NavMeshAgent _Agent;

    void Awake()
    {
        _Agent = GetComponent<NavMeshAgent>();
    }

    public BaseArea FindNearestArea()
    {
        List<BaseArea> playerAreas = new List<BaseArea>();
        foreach(BaseArea area in BaseAreaManager.Instance.BaseAreaList)
        {
            if (area.BelongSide == Side.Team1)
                playerAreas.Add(area);
        }

        float minDis = float.MaxValue;
        BaseArea baseArea = null;
        foreach(BaseArea area in playerAreas)
        {
            float dis = DistanceToBaseArea(area);
            if (dis < minDis)
            {
                minDis = dis;
                baseArea = area;
            }
        }

        return baseArea;
    }

    float DistanceToBaseArea(BaseArea area)
    {
        NavMeshPath path = new NavMeshPath();
        _Agent.CalculatePath(area.Center.position, path);
        return PathLength(path);
    }

    /// <summary>
    /// 计算路径的长度
    /// </summary>
    float PathLength(NavMeshPath path)
    {
        if (path.corners.Length < 2)
            return 0;

        Vector3 previousCorner = path.corners[0];
        float lengthSoFar = 0.0F;
        int i = 1;
        while (i < path.corners.Length)
        {
            Vector3 currentCorner = path.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
            previousCorner = currentCorner;
            i++;
        }
        return lengthSoFar;
    }
}
