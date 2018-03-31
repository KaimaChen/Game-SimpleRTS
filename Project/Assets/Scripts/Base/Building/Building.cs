using UnityEngine;

public class Building : MonoBehaviour {
    protected BuildingBase mOwner;

    protected virtual void Start()
    {
        mOwner = transform.parent.parent.GetComponent<BuildingBase>();
    }

    public Side BelongSide()
    {
        if(mOwner == null)
            mOwner = transform.parent.parent.GetComponent<BuildingBase>();

        return mOwner.BelongSide;
    }

	public void BeingHit(float demage)
    {
        mOwner.DecreaseHp(demage);
    }
}
