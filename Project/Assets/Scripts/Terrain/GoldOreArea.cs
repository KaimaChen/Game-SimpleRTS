using UnityEngine;
using System.Collections.Generic;

public class GoldOreArea : MonoBehaviour {
    public static List<GoldOreArea> GoldOreAreaList = new List<GoldOreArea>();

    public Transform NavTarget; //定义该金矿在导航中的位置，不用中心是因为可能会在Carve内而导致计算导航路径长度时错误

    private List<GoldOre> mGoldOres = new List<GoldOre>();
    public List<GoldOre> GoldOres
    {
        get { return mGoldOres; }
    }

    void Awake()
    {
        NavTarget = transform.FindChild("NavTarget");
        GoldOreAreaList.Add(this);
    }

	void Start () {
        GoldOre[] golds = GetComponentsInChildren<GoldOre>();
        for(int i = 0; i < golds.Length; i++)
        {
            golds[i].Owner = this;
            mGoldOres.Add(golds[i]);
        }
    }

    void OnDestroy()
    {
        GoldOreAreaList.Remove(this);
    }

    public void Remove(GoldOre ore)
    {
        mGoldOres.Remove(ore);
        if (mGoldOres.Count <= 0)
            Destroy(gameObject);
    }
}
