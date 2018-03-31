using UnityEngine;
using System.Collections;

public class RoundProcess : MonoBehaviour {
    public float FillTime = 2; //从零到填满所需时间
    public bool IsFilling = false;

    private float _Timer = 0;
    private Material _Mat;

    public delegate void VoidEventHandler();
    public event VoidEventHandler FillEvent;

    void Awake()
    {
        _Mat = GetComponent<MeshRenderer>().sharedMaterial;
        SetProcess(0);
    }
    
	void Update () {
	    if(IsFilling)
        {
            _Timer += Time.deltaTime;
            float p = _Timer / FillTime;
            SetProcess(p);
            if(p >= 1 && FillEvent != null)
            {
                FillEvent.Invoke();
            }
        }
	}

    public  void Reset()
    {
        _Timer = 0;
        IsFilling = false;
    }

    public void SetProcess(float p)
    {
        p = Mathf.Clamp(p, 0, 1);
        _Mat.SetFloat("_Progress", p);
    }
}
