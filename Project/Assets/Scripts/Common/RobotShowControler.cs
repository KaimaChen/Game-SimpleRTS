using UnityEngine;
using System.Collections.Generic;

public class RobotShowControler : MonoBehaviour {
    public static RobotShowControler Instance = null;

    List<GameObject> mWeaponParts = new List<GameObject>();
    List<GameObject> mChassisParts = new List<GameObject>();
    Dictionary<WeaponType, GameObject> mWeapons = new Dictionary<WeaponType, GameObject>();
    Dictionary<WeaponType, GameObject> mNoneWeapons = new Dictionary<WeaponType, GameObject>();
    Dictionary<ChassisType, GameObject> mChassis = new Dictionary<ChassisType, GameObject>();
    Dictionary<ChassisType, GameObject> mNoneChassis = new Dictionary<ChassisType, GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

	void Start () {
        AddWeapons(WeaponType.Gun);
        AddWeapons(WeaponType.Laser);
        AddWeapons(WeaponType.Cannon);
        AddWeapons(WeaponType.Rocket);
        AddChassis(ChassisType.Wheels);
        AddChassis(ChassisType.Hover);
        AddChassis(ChassisType.Legs);
        AddChassis(ChassisType.Tracks);
	}
    
    public void ShowWeapon(WeaponType weaponType, bool isWeaponReady)
    {
        for (int i = 0; i < mWeaponParts.Count; i++)
            mWeaponParts[i].SetActive(false);

        if (isWeaponReady)
            mWeapons[weaponType].SetActive(true);
        else
            mNoneWeapons[weaponType].SetActive(true);
    }

    public void ShowChassis(ChassisType chassisType, bool isChassisReady)
    {
        for (int i = 0; i < mChassisParts.Count; i++)
            mChassisParts[i].SetActive(false);

        if (isChassisReady)
            mChassis[chassisType].SetActive(true);
        else
            mNoneChassis[chassisType].SetActive(true);
    }
	
    void AddWeapons(WeaponType type)
    {
        GameObject go = FindWeapon(type);
        mWeaponParts.Add(go);
        mWeapons.Add(type, go);

        go = FindNoneWeapon(type);
        mWeaponParts.Add(go);
        mNoneWeapons.Add(type, go);
    }

    void AddChassis(ChassisType type)
    {
        GameObject go = FindChassis(type);
        mChassisParts.Add(go);
        mChassis.Add(type, go);

        go = FindNoneChassis(type);
        mChassisParts.Add(go);
        mNoneChassis.Add(type, go);
    }

    GameObject FindWeapon(WeaponType weaponType)
    {
        return transform.FindChild("Weapon/" + weaponType.ToString()).gameObject;
    }

    GameObject FindNoneWeapon(WeaponType weaponType)
    {
        return transform.FindChild("Weapon/" + weaponType.ToString() + "_None").gameObject;
    }

    GameObject FindChassis(ChassisType chassisType)
    {
        return transform.FindChild("Chassis/" + chassisType.ToString()).gameObject;
    }

    GameObject FindNoneChassis(ChassisType chassisType)
    {
        return transform.FindChild("Chassis/" + chassisType.ToString() + "_None").gameObject;
    }
}
