using UnityEngine;
using System.Collections.Generic;
using System.Xml;

public class GameConfig : MonoBehaviour {
    public static GameConfig Instance = null;

    public const int NAV_WALKABLE = 0;
    public const int NAV_WATER = 3;

    public TextAsset dataFile;
    public Transform LeftUp;
    public Transform RightBottom;

    private XmlDocument mDoc;
    private Dictionary<string, float> mData = new Dictionary<string, float>();

    #region Property
    private Vector2 mSceneHorizontal;
    public Vector2 SceneHorizontal
    {
        get { return mSceneHorizontal; }
    }

    private Vector2 mSceneVertical;
    public Vector2 SceneVertical
    {
        get { return mSceneVertical; }
    }

    private float mSceneWidth;
    public float SceneWidth
    {
        get { return mSceneWidth; }
    }

    private float mSceneHeight;
    public float SceneHeight
    {
        get { return mSceneHeight; }
    }

    private float mNeighborDistance;
    public float NeighBorDistance
    {
        get { return mNeighborDistance; }
    }

    private int mOriginMoney;
    public int OriginMoney
    {
        get { return mOriginMoney; }
    }

    private int mBuildingSpeed;
    public int BuildingSpeed
    {
        get { return mBuildingSpeed; }
    }

    private int mBuildingMaxHp;
    public int BuildingMaxHp
    {
        get { return mBuildingMaxHp; }
    }

    private float mEnemySelectTime;
    public float EnemySelectTime
    {
        get { return mEnemySelectTime; }
    }
    #endregion

    void Awake()
    {
        if (Instance == null)
        {
            InitData();
            Instance = this;
        }
        else
            Destroy(gameObject);
    }
    
    void InitData()
    {
        mSceneHorizontal = new Vector2(LeftUp.position.x, RightBottom.position.x);
        mSceneVertical = new Vector2(RightBottom.position.z, LeftUp.position.z);
        mSceneWidth = mSceneHorizontal.y - mSceneHorizontal.x;
        mSceneHeight = mSceneVertical.y - mSceneVertical.x;

        mDoc = new XmlDocument();
        mDoc.LoadXml(dataFile.text);

        AnalysisXmlNode("Data/Weapon");
        AnalysisXmlNode("Data/Chassis");
        AnalysisXmlNode("Data/Building");
        mNeighborDistance = float.Parse(GetNodeValue("Data/NeighborDistance"));
        mOriginMoney = int.Parse(GetNodeValue("Data/OriginMoney"));
        mBuildingSpeed = int.Parse(GetNodeValue("Data/BuildSpeed"));
        mBuildingMaxHp = int.Parse(GetNodeValue("Data/BuildingMaxHp"));
        mEnemySelectTime = float.Parse(GetNodeValue("Data/EnemySelectTime"));
    }

    void AnalysisXmlNode(string path)
    {
        XmlNode containerNode = mDoc.SelectSingleNode(path);
        XmlNodeList list = containerNode.ChildNodes;
        foreach (XmlNode node in list)
        {
            string weaponName = node.Name;
            foreach (XmlNode propertyNode in node.ChildNodes)
            {
                float value = float.Parse(propertyNode.InnerText);
                mData.Add(weaponName + propertyNode.Name, value);
            }
        }
    }

    string GetNodeValue(string path)
    {
        return mDoc.SelectSingleNode(path).InnerText;
    }
    
    public float GetAttackRate(WeaponType type)
    {
        return mData[type.ToString() + "AttackRate"];
    }

    public float GetAttackRadius(WeaponType type)
    {
        return mData[type.ToString() + "AttackRadius"];
    }

    public float GetDamage(WeaponType type)
    {
        return mData[type.ToString() + "Damage"];
    }

    public float GetBulletSpeed(WeaponType type)
    {
        return mData[type.ToString() + "Speed"];
    }

    public float GetWeaponPrice(WeaponType type)
    {
        return mData[type.ToString() + "Price"];
    }

    public float GetArmor(ChassisType type)
    {
        return mData[type.ToString() + "Armor"];
    }

    public float GetMoveSpeed(ChassisType type)
    {
        return mData[type.ToString() + "Speed"];
    }

    public float GetChassisPrice(ChassisType type)
    {
        return mData[type.ToString() + "Price"];
    }

    public float GetBuildingPrice(BuildingType buildType, WeaponType weaponType = WeaponType.None, ChassisType chassisType = ChassisType.None)
    {
        if(buildType == BuildingType.RobotPart)
        {
            if (weaponType != WeaponType.None)
                return mData[weaponType.ToString() + "BuildingPrice"];
            else if (chassisType != ChassisType.None)
                return mData[chassisType.ToString() + "BuildingPrice"];
            else
                return 0;
        }
        else if(buildType == BuildingType.None)
        {
            return 0;
        }
        else
        {
            return mData[buildType.ToString() + "Price"];
        }
    }

    public Vector3 TopLeft()
    {
        return new Vector3(mSceneHorizontal.x, 0, mSceneVertical.y);
    }

    public Vector3 TopRight()
    {
        return new Vector3(mSceneHorizontal.y, 0, mSceneVertical.y);
    }

    public Vector3 BottomLeft()
    {
        return new Vector3(mSceneHorizontal.x, 0, mSceneVertical.x);
    }

    public Vector3 BottomRight()
    {
        return new Vector3(mSceneHorizontal.y, 0, mSceneVertical.x);
    }
}
