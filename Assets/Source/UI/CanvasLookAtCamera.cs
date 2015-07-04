using UnityEngine;
using System.Collections;

public class CanvasLookAtCamera : MonoBehaviour {

	protected virtual void Start () {
	}
	
	// Update is called once per frame
	protected virtual void LateUpdate ()
    {
        Quaternion cameraRotation = Game.Get().playerCamera.transform.rotation;
        if(transform.rotation != cameraRotation){
            transform.rotation = cameraRotation;
        }
    }
}
