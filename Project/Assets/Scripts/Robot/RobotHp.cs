using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RobotData))]
public class RobotHp : MonoBehaviour {
    private Transform _Hp;
    private RobotData _Data;

    void Awake()
    {
        _Hp = transform.FindChild("Hp");
        _Data = GetComponent<RobotData>();
    }
    
	void Update () {
        _Hp.localScale = new Vector3(1, 1, _Data.HpPercent());
	}
}
