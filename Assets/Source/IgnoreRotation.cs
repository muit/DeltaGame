using UnityEngine;
using System.Collections;

public class IgnoreRotation : MonoBehaviour {
    private Quaternion firstRotation;

    void Start() {
        firstRotation = transform.rotation;
    }
	void LateUpdate () {
        transform.rotation = firstRotation;
	}
}
