using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform Target;
    public float RotateSpeed = 0.6f;
    public float ZoomSpeed = 0.1f;
    public float LerpSpeed = 10f;
    public float AutoRotateSpeed = 0.6f;
    public float FarthestDistance = 30f;
    public float NearestDistance = 3f;
    public float ScreenMiddle = 0.5f;
    public float DefaultTapTime = 0.2f;

    private Transform _cameraPivot;

    // Maintain current rotation.
    private float verticalRotation;
    private float horizontalRotation;

    [SerializeField]
    private InputAction _rotate, _zoom;

    private void Awake()
    {
        _cameraPivot = gameObject.transform.parent;
    }

    private void OnEnable()
    {
        _rotate.Enable();
        _zoom.Enable();
    }

    private void OnDisable()
    {
        _rotate.Disable();
        _zoom.Disable();
    }

    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        // Follow Player
        Vector3 newPivotPosition = Vector3.Lerp(_cameraPivot.position, Target.position, LerpSpeed * Time.deltaTime);
        Vector3 curVec = _cameraPivot.forward;
        Vector3 nextVec = curVec + (newPivotPosition - _cameraPivot.position);
        float signedAngle = Vector2.SignedAngle(new Vector2(curVec.x, curVec.z), new Vector2(nextVec.x, nextVec.z));
        float autoRotateAngle = -signedAngle * AutoRotateSpeed;
        _cameraPivot.position = newPivotPosition;
        Rotate(new Vector2(autoRotateAngle, 0));
    }

    private void Update()
    {
        UpdateRotate();
        UpdateZoom();
    }

    private void UpdateRotate()
    {
        Vector2 touchDelta = _rotate.ReadValue<Vector2>();
        touchDelta = ScreenScale(touchDelta);
        Vector2 angle_rotate = touchDelta * RotateSpeed;
        Rotate(angle_rotate);
    }

    private void Rotate(Vector2 angle)
    {
        // Calculate New Rotation
        verticalRotation -= angle.y;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
        horizontalRotation += angle.x;
        _cameraPivot.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }
    
    private void UpdateZoom()
    {

        float distDelta = _zoom.ReadValue<float>();
        Zoom(distDelta * ZoomSpeed);
    }

    private void Zoom(float distDelta)
    {
        // New Camera Position
        Vector3 newCamPos = transform.localPosition;
        newCamPos -= distDelta * newCamPos.normalized;

        // Clamp Magnitude
        float dist = newCamPos.magnitude;
        if (newCamPos.z > 0) // Disable Negative
        {
            newCamPos = -newCamPos.normalized * NearestDistance;
        }
        if (dist > FarthestDistance)
        {
            newCamPos = newCamPos.normalized * FarthestDistance;
        }
        else if (dist < NearestDistance)
        {
            newCamPos = newCamPos.normalized * NearestDistance;
        }

        // Update sprimg arm (by change camera position)
        transform.localPosition = newCamPos;
    }

    private Vector2 ScreenScale(Vector2 vec)
    {
        // Scale vector like we do in Canvas Scaler.
        // Reference Resolution : (width, height) = (800, 600)
        Vector2 ret = vec;
        ret.x *= 800f / Screen.width;
        ret.y *= 600f / Screen.height;
        return ret;
    }
}