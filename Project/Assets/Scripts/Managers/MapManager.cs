using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance = null;
    
    public GameObject CameraRectPrefab;
    
    private Transform mMainCamera;
    private Transform mCameraRect;
    private RectTransform mRect;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        mMainCamera = Camera.main.transform;
        mCameraRect = (Instantiate(CameraRectPrefab) as GameObject).transform;
        mCameraRect.SetParent(transform);
        mCameraRect.localScale = Vector3.one;
        mRect = GetComponent<RectTransform>();
    }

    public void Update()
    {
        mCameraRect.localPosition = WorldPos2MapPos(mMainCamera.position);
    }

	public Vector2 WorldPos2MapPos(Vector3 worldPos)
    {
        Vector2 sceneHorizontal = GameConfig.Instance.SceneHorizontal;
        float x = Mathf.Clamp(worldPos.x, sceneHorizontal.x, sceneHorizontal.y);
        float xPercent = (x - sceneHorizontal.x) / (sceneHorizontal.y - sceneHorizontal.x);

        Vector2 sceneVertical = GameConfig.Instance.SceneVertical;
        float y = Mathf.Clamp(worldPos.z, sceneVertical.x, sceneVertical.y);
        float yPercent = (y - sceneVertical.x) / (sceneVertical.y - sceneVertical.x);
        
        return new Vector2(mRect.sizeDelta.x * xPercent, mRect.sizeDelta.y * yPercent);
    }
}
