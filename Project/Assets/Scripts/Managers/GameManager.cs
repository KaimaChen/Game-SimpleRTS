using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance = null;
    public int CheckFrequency = 10;

    private int mFrameCounter;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        mFrameCounter = CheckFrequency;
    }

    void Update()
    {
        mFrameCounter++;
        if(mFrameCounter >= CheckFrequency)
        {
            mFrameCounter = 0;
            CheckWin();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnApplicationPause(true);
        }
    }
    
	void OnApplicationPause(bool state)
    {
        if(state)
        {
            UIManager.Instance.OpenPausePage();
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    void CheckWin()
    {
        if(BaseAreaManager.Instance.EnemyAreaCount() == 0)
        {
            SceneManager.LoadScene("Win");
        }
        else if(BaseAreaManager.Instance.PlayerAreaCount() == 0)
        {
            SceneManager.LoadScene("Fail");
        }
    }
}
