using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour {
    public float SenseRadius = 10f;
    public List<Vector3> WayPoints = new List<Vector3>();

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, SenseRadius);
    }

    /// <summary>
    /// 创建某个区域的巡逻路径
    /// </summary>
    /// <param name="area"></param>
    public void CreateWayPointsIn(BaseArea area)
    {
        WayPoints.Clear();

        /*巡逻路径如下
         *      *
             * 
         *      *
        */
        float radius = area.SenseRadius;
        Vector3 center = area.Center.position;
        Vector3 topLeft = center + new Vector3(-radius, 0, radius);
        Vector3 topRight = center + new Vector3(radius, 0, radius);
        Vector3 bottomLeft = center + new Vector3(-radius, 0, -radius);
        Vector3 bottomRight = center + new Vector3(radius, 0, -radius);

        WayPoints.Add(center);
        WayPoints.Add(topLeft);
        WayPoints.Add(topRight);
        WayPoints.Add(bottomLeft);
        WayPoints.Add(bottomRight);
    }
}
