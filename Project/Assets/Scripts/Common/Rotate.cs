using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
    public Vector3 Rotation = new Vector3(0, 0, 20);
    
	void Update () {
        this.transform.Rotate(Rotation * Time.deltaTime);
	}
}
