using UnityEngine;

public class RobotInteraction : MonoBehaviour {
    [SerializeField]
    private GameObject mSelectedGo;
    private float mDuration;
    private float mTimer;

    private bool mIsSelected = false;
    public bool IsSelected
    {
        get { return mIsSelected; }
        set { mIsSelected = value; }
    }

    void Start()
    {
        Deselect();
    }

    void Update()
    {
        if (mDuration >= 0)
        {
            mTimer += Time.deltaTime;
            if (mTimer >= mDuration)
                Deselect();
        }
    }

    public void Select(float duration = -1)
    {
        IsSelected = true;
        mDuration = duration;
        mSelectedGo.SetActive(true);
    }

    public void Deselect()
    {
        IsSelected = false;
        mDuration = -1;
        mSelectedGo.SetActive(false);
    }
}
