using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    public float distance = 10;
    public float inclination = 45;
    public float yRotation = 180;
    public float moveDamping = 3;
    public float teleportAtDistance = 100;

    private Transform spawn;
    private Transform target;

    [System.NonSerialized]
    public MotionBlur motionBlur;

    void Start() {
        motionBlur = GetComponent<MotionBlur>();
        target = transform.FindChild("Target");
    }

	void Update () {
        UpdatePosition(Game.Get().GetTarget());
    }

    private void UpdatePosition(Transform trans)
    {
        if (!trans)
        {
            Debug.LogWarning("Camera target transform is null.");
            return;
        }

        Vector3 position = trans.position;
        position.y += distance * Mathf.Sin((90 - inclination) * 2 * Mathf.PI / 360);

        float xzDistance = distance * Mathf.Cos((90 - inclination) * 2 * Mathf.PI / 360);
        position.x += xzDistance * Mathf.Sin(yRotation * 2 * Mathf.PI / 360);
        position.z += xzDistance * Mathf.Cos(yRotation * 2 * Mathf.PI / 360);

        transform.LookAt(trans);

        //Teleport camera if is too far
        if (Vector3.Distance(trans.position, transform.position) > teleportAtDistance)
            transform.position = position;
        else
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * moveDamping);
    }
}
