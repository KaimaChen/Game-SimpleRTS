using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 管理所有特殊地形，比如金矿区
/// </summary>
public class TerrainManager : MonoBehaviour {
    public static TerrainManager Instance = null;

    private List<GoldOreArea> mGoldOreAreas;
    public List<GoldOreArea> GoldOreAreas
    {
        get { return mGoldOreAreas; }
    }
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        GoldOreArea[] areas = gameObject.GetComponentsInChildren<GoldOreArea>();
        mGoldOreAreas = new List<GoldOreArea>(areas);
    }
}
