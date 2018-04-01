using UnityEngine;

public class ResourcesManager : MonoBehaviour {
    public static ResourcesManager Instance = null;
    
    void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public GameObject GetPrefab(string name)
    {
        return Resources.Load(name) as GameObject;
    }

    public RenderTexture GetRT(string name)
    {
        return Resources.Load("RT/" + name) as RenderTexture;
    }

    public RuntimeAnimatorController GetAnimController(string name)
    {
        return Resources.Load("FSM/" + name) as RuntimeAnimatorController;
    }
}
