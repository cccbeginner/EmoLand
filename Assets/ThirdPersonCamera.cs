using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform Target;
    public float MouseSensitivity = 10f;

    private Transform _cameraPivot;

    private float verticalRotation;
    private float horizontalRotation;

    private void Awake()
    {
        _cameraPivot = gameObject.transform.parent;
    }

    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        _cameraPivot.position = Target.position;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        verticalRotation -= mouseY * MouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

        horizontalRotation += mouseX * MouseSensitivity;

        _cameraPivot.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }
}