using UnityEngine;
using System.Collections;

public class Tools {
    public static float Distance(Vector3 p1, Vector3 p2)
    {
        Vector2 v1 = new Vector2(p1.x, p1.z);
        Vector2 v2 = new Vector2(p2.x, p2.z);
        return Vector2.Distance(v1, v2);
    }

    public static void RemoveAllChildren(Transform parent)
    {
        for(int i = 0; i < parent.childCount; i++)
        {
            GameObject.Destroy(parent.GetChild(i));
        }
    }
}
