using UnityEngine;
using System.Collections.Generic;

public class GoldOreArea : MonoBehaviour {
    private List<GoldOre> mGoldOres = new List<GoldOre>(); 

    public List<GoldOre> GoldOres
    {
        get { return mGoldOres; }
    }

	void Start () {
        GoldOre[] golds = GetComponentsInChildren<GoldOre>();
        for(int i = 0; i < golds.Length; i++)
        {
            golds[i].Owner = this;
            mGoldOres.Add(golds[i]);
        }
    }
	
    public void Remove(GoldOre target)
    {
        mGoldOres.Remove(target);
    }
}
