using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {
    public Vector3 DeltaPerFrame = new Vector3(0, 1f, 0);

	void Update () {
        transform.localEulerAngles += DeltaPerFrame;
	}
}
