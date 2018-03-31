using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
    public const float SENSITY = 0.05F;

    public Vector2 HorizontalBorder;
    public Vector2 VerticalBorder;
    
    public void Move(Vector3 delta)
    {
        Vector3 pos = transform.position;

        float x = pos.x + delta.x;
        x = Mathf.Clamp(x, HorizontalBorder.x, HorizontalBorder.y);
        float z = pos.z + delta.z;
        z = Mathf.Clamp(z, VerticalBorder.x, VerticalBorder.y);
        transform.position = new Vector3(x, pos.y, z);
    }
}
