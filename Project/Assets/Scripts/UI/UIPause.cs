using UnityEngine;
using UnityEngine.UI;

public class UIPause : MonoBehaviour {
    Button mBtnResume;
    Button mBtnClose;

    void Awake()
    {
        mBtnResume = transform.FindChild("BtnResume").GetComponent<Button>();
        mBtnClose = transform.FindChild("BtnClose").GetComponent<Button>();

        mBtnResume.onClick.AddListener(OnClickResume);
        mBtnClose.onClick.AddListener(OnClickClose);
    }

    void OnClickResume()
    {
        gameObject.SetActive(false);
        UIManager.Instance.ShowMap();
        GameManager.Instance.ResumeGame();
    }

    void OnClickClose()
    {
        Application.Quit();
    }
}
