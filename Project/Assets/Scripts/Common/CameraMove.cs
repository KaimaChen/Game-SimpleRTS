using UnityEngine;

public class CameraMove : MonoBehaviour {
    public const float SENSITY = 0.05F;
    
    public void Move(Vector3 delta)
    {
        Vector3 pos = transform.position;

        float x = pos.x + delta.x;
        GameConfig config = GameConfig.Instance;
        x = Mathf.Clamp(x, config.SceneHorizontal.x, config.SceneHorizontal.y);
        float z = pos.z + delta.z;
        z = Mathf.Clamp(z, config.SceneVertical.x, config.SceneVertical.y);
        transform.position = new Vector3(x, pos.y, z);
    }
}
