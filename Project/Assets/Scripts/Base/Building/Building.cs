using UnityEngine;

public class Building : MonoBehaviour {
    protected BuildingBase mOwner;

    protected virtual void Start()
    {
        mOwner = GetComponentInParent<BuildingBase>();
    }

    public Side BelongSide()
    {
        if(mOwner == null)
            mOwner = GetComponentInParent<BuildingBase>();

        return mOwner.BelongSide;
    }

	public void BeingHit(float demage)
    {
        mOwner.DecreaseHp(demage);
    }
}
