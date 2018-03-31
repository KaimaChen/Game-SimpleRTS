using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// 统一管理输入
/// </summary>
public class InputManager : MonoBehaviour {
    public static InputManager Instance = null;

    public CameraMove CamMove;

    public delegate void VoidEventHandler();
    public event VoidEventHandler LeftClickDownEvent;
    public event VoidEventHandler LeftClickUpEvent;

    private bool mIsPressing = false;
    private Vector3 mLastPressPos = Vector3.zero;
    private Vector3 mLastCameraPos = Vector3.zero;

    [SerializeField]
    private bool mIsOpen = true;
    public bool IsOpen
    {
        get { return mIsOpen; }
        set { mIsOpen = value; }
    }
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
	void Update () {
        if (mIsOpen == false || EventSystem.current.IsPointerOverGameObject())
        {
            mIsPressing = false;
            return;
        }

        MouseClick();
        MoveCamera();
	}

    void MouseClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            mIsPressing = true;
            mLastPressPos = Input.mousePosition;
            mLastCameraPos = CamMove.transform.position;

            if(LeftClickDownEvent != null)
            {
                LeftClickDownEvent.Invoke();
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            mIsPressing = false;

            float camMoveDis = Vector3.Magnitude(CamMove.transform.position - mLastCameraPos);
            if(LeftClickUpEvent != null && camMoveDis < 2) //移动了摄像机则不进行机器人的移动
            {
                LeftClickUpEvent.Invoke();
            }
        }
    }

    void MoveCamera()
    {
        if (mIsPressing == false) return;

        Vector3 delta = mLastPressPos - Input.mousePosition;
        mLastPressPos = Input.mousePosition;
        delta *= CameraMove.SENSITY;
        delta = new Vector3(delta.x, 0, delta.y);
        CamMove.Move(delta);
    }
}
