using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIStartPage : MonoBehaviour {
    public Button btnStart;
    public Button btnExit;

	void Start () {
        btnStart.onClick.AddListener(OnClickStart);
        btnExit.onClick.AddListener(OnClickExit);
	}
	
    void OnClickStart()
    {
        SceneManager.LoadScene("Main");
    }

    void OnClickExit()
    {
        Application.Quit();
    }
}
