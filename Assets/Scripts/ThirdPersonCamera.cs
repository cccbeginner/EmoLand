using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform Target;
    public float HorizontalRotateSpeed = 0.6f;
    public float VerticalRotateSpeed = 0.3f;
    public float ZoomSpeed = 0.1f;
    public float LerpSpeed = 10f;
    public float AutoRotateSpeed = 0.6f;
    public float FarthestDistance = 30f;
    public float NearestDistance = 3f;
    public float ScreenMiddle = 0.5f;
    public float DefaultTapTime = 0.2f;

    public Transform m_CameraPivot;

    // Maintain current rotation.
    private float m_VerticalRotation;
    private float m_HorizontalRotation;
    private float m_PinchCameraDistance;

    [SerializeField]
    private InputAction m_Rotate, m_Zoom;

    private void Awake()
    {
        transform.parent = m_CameraPivot;
        m_PinchCameraDistance = transform.localPosition.magnitude;
    }

    private void OnEnable()
    {
        m_Rotate.Enable();
        m_Zoom.Enable();
    }

    private void OnDisable()
    {
        m_Rotate.Disable();
        m_Zoom.Disable();
    }

    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        // Follow Player
        Vector3 newPivotPosition = Vector3.Lerp(m_CameraPivot.position, Target.position, LerpSpeed * Time.deltaTime);
        Vector3 curVec = m_CameraPivot.forward;
        Vector3 nextVec = curVec + (newPivotPosition - m_CameraPivot.position);
        float signedAngle = Vector2.SignedAngle(new Vector2(curVec.x, curVec.z), new Vector2(nextVec.x, nextVec.z));
        float autoRotateAngle = -signedAngle * AutoRotateSpeed;
        m_CameraPivot.position = newPivotPosition;
        Rotate(new Vector2(autoRotateAngle, 0));
    }

    private void Update()
    {
        UpdateRotate();
        UpdateZoom();
    }

    private void UpdateRotate()
    {
        Vector2 touchDelta = m_Rotate.ReadValue<Vector2>();
        Vector2 angle_rotate = ScreenScale(touchDelta);
        angle_rotate.x *= HorizontalRotateSpeed;
        angle_rotate.y *= VerticalRotateSpeed;
        Rotate(angle_rotate);
    }

    private void Rotate(Vector2 angle)
    {
        // Calculate New Rotation
        m_VerticalRotation -= angle.y;
        m_VerticalRotation = Mathf.Clamp(m_VerticalRotation, -80f, 80f);
        m_HorizontalRotation += angle.x;
        m_CameraPivot.rotation = Quaternion.Euler(m_VerticalRotation, m_HorizontalRotation, 0);
    }
    
    private void UpdateZoom()
    {

        float pinchDelta = m_Zoom.ReadValue<float>();
        m_PinchCameraDistance -= pinchDelta * ZoomSpeed;
        m_PinchCameraDistance = Mathf.Clamp(m_PinchCameraDistance, NearestDistance, FarthestDistance);

        // Camera collider ray
        int layerMask = LayerMask.GetMask("Water", "Static Object");
        bool isRayHit = Physics.Raycast(
            m_CameraPivot.position,
            (transform.position - m_CameraPivot.position).normalized, 
            out RaycastHit hit,
            m_PinchCameraDistance,
            layerMask
        );

        float curDist = transform.localPosition.magnitude;
        float targetDist = m_PinchCameraDistance;
        if (isRayHit)
        {
            // Override pinch distance
            targetDist = hit.distance;
        }
        float smoothDist = Mathf.Lerp(curDist, targetDist, LerpSpeed * Time.deltaTime);
        SetCameraDist(smoothDist);
    }

    // Return result distance;
    private float SetCameraDist(float newDist)
    {
        // Clamp Magnitude
        newDist = Mathf.Clamp(newDist, NearestDistance, FarthestDistance);

        // Update sprimg arm (by change camera position)
        Vector3 newCamPos = newDist * transform.localPosition.normalized;
        transform.localPosition = newCamPos;
        return newDist;
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