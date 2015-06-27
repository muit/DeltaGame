using UnityEngine;
using System.Collections;

public class IgnoreRotation : MonoBehaviour {
	void LateUpdate () {
        transform.rotation = Quaternion.identity;
	}
}
