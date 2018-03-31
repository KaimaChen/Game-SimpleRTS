using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIBuildingStatus : MonoBehaviour {
    private Button mBtnClose;
    private Button mBtnSell;
    private Text mTitle;
    private Text mHp;
    private RawImage mModelImg;
    
    private BuildingBase mSelectedBase = null;

    void Awake()
    {
        mBtnClose = transform.FindChild("BtnClose").GetComponent<Button>();
        mBtnSell = transform.FindChild("BtnSell").GetComponent<Button>();
        mTitle = transform.FindChild("Title").GetComponent<Text>();
        mHp = transform.FindChild("Hp").GetComponent<Text>();
        mModelImg = transform.FindChild("Model").GetComponent<RawImage>();

        mBtnClose.onClick.AddListener(ClosePage);
        mBtnSell.onClick.AddListener(OnClickSell);
    }

    public void InitView(BuildingBase selectedBase)
    {
        mSelectedBase = selectedBase;
        if (mSelectedBase == null)
            return;
        
        BuildingBase b = mSelectedBase.GetComponent<BuildingBase>();
        b.ChangeHpEvent += RefreshHp;
        b.BuildDoneEvent += OnBuildDone;
        mTitle.text = b.Name;
        mHp.text = b.HpText();

        string name = string.Empty;
        if (b.IsBuildingNow)
        {
            name = "NotCompleteBuilding";
        }
        else
        {
            if(b.BuildingType != BuildingType.None && b.BuildingType != BuildingType.RobotPart)
            {
                name = b.BuildingType.ToString();
            }
            else if(b.BuildingType == BuildingType.RobotPart)
            {
                name = (b.WeaponType == WeaponType.None) ? b.ChassisType.ToString() : b.WeaponType.ToString();
            }
        }
        mModelImg.texture = ResourcesManager.Instance.GetRT(name);
    }
    
    void RefreshHp()
    {
        BuildingBase b = mSelectedBase.GetComponent<BuildingBase>();
        mTitle.text = b.Name;
        mHp.text = b.HpText();
    }
    
    void ClosePage()
    {
        gameObject.SetActive(false);

        BuildingBase b = mSelectedBase.GetComponent<BuildingBase>();
        b.ChangeHpEvent -= RefreshHp;
        b.BuildDoneEvent -= OnBuildDone;
        mSelectedBase = null;

        InputManager.Instance.IsOpen = true;
    }

    void OnClickSell()
    {
        BuildingBase b = mSelectedBase.GetComponent<BuildingBase>();
        b.SellBuilding();
        ClosePage();
    }

    void OnBuildDone()
    {
        string name = string.Empty;
        if (mSelectedBase.BuildingType != BuildingType.None && mSelectedBase.BuildingType != BuildingType.RobotPart)
        {
            name = mSelectedBase.ToString();
        }
        else if (mSelectedBase.BuildingType == BuildingType.RobotPart)
        {
            name = (mSelectedBase.WeaponType == WeaponType.None) ? mSelectedBase.ChassisType.ToString() : mSelectedBase.WeaponType.ToString();
        }
        mModelImg.texture = ResourcesManager.Instance.GetRT(name);
    }
}
