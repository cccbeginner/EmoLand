using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public float HorizontalRotateSpeed = 0.6f;
    public float VerticalRotateSpeed = 0.3f;
    public float ZoomSpeed = 0.1f;
    public float MoveSpeed = 10f;
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

    private Vector3 m_PivotOffset = Vector3.zero;

    // true if the camera is being dragged currently
    private bool m_IsDragging = false;
    public UnityEvent OnDraggedStart;
    public UnityEvent OnDraggedEnd;

    public static ThirdPersonCamera main { get; private set; }

    private Vector3 m_InitPos;
    private Quaternion m_InitRot;

    private void Awake()
    {
        m_PivotOffset = m_CameraPivot.localPosition;
        transform.parent = m_CameraPivot;
        m_PinchCameraDistance = transform.localPosition.magnitude;
        main = this;
        m_InitPos = transform.localPosition;
        m_InitRot = transform.localRotation;
    }

    private void OnEnable()
    {
        m_Rotate.Enable();
        m_Zoom.Enable();
        m_IsDragging = false;
    }

    private void OnDisable()
    {
        m_Rotate.Disable();
        m_Zoom.Disable();
        StopAllCoroutines();
    }

    void LateUpdate()
    {
        if (Player.main == null)
        {
            return;
        }

        // Follow Player
        if (!m_IsDragging)
        {
            // Rotate & position pivot
            Vector3 newPivotPosition = Vector3.Lerp(m_CameraPivot.position, Player.main.transform.position + m_PivotOffset, MoveSpeed * Time.deltaTime);
            Vector3 curVec = m_CameraPivot.forward;
            Vector3 nextVec = curVec + (newPivotPosition - m_CameraPivot.position);
            float signedAngle = Vector2.SignedAngle(new Vector2(curVec.x, curVec.z), new Vector2(nextVec.x, nextVec.z));
            float autoRotateAngle = -signedAngle * AutoRotateSpeed;
            m_CameraPivot.position = newPivotPosition;
            Rotate(new Vector2(autoRotateAngle, 0));
        }
    }

    private void Update()
    {
        if (!m_IsDragging)
        {
            UpdateRotate();
            UpdateZoom();
        }
    }

    public void SetRotation(Vector3 eulerDirection)
    {
        m_VerticalRotation = eulerDirection.x;
        m_HorizontalRotation = eulerDirection.y;
        m_CameraPivot.rotation = Quaternion.Euler(eulerDirection);
        m_CameraPivot.position = Player.main.transform.position + m_PivotOffset;
        transform.localRotation = m_InitRot;
        transform.localPosition = m_InitPos;
    }

    public void DragCameraToTransform(Transform targetTransform, float timeStay)
    {
        // The condition prevents 2 drags of camera
        if (!m_IsDragging) {
            StartCoroutine(DragCameraAction(targetTransform, timeStay));
        }
    }

    IEnumerator DragCameraAction(Transform targetTransform, float timeStay)
    {
        m_IsDragging = true;
        OnDraggedStart.Invoke();

        const float transportTime = 2f;
        float timeNow = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        // Drag the camera to the target transform
        while (timeNow <= timeStay && timeNow / transportTime <= 1)
        {
            timeNow += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetTransform.position, Mathf.SmoothStep(0, 1, timeNow / transportTime));
            transform.rotation = Quaternion.Lerp(startRot, targetTransform.rotation, Mathf.SmoothStep(0, 1, timeNow / transportTime));
            yield return null;
        }

        // Let camera stay for awhile 
        while(timeNow <= timeStay)
        {
            timeNow += Time.deltaTime;
            yield return null;
        }

        // Drag camera to the original transform
        // (Put away the camera)
        timeNow = 0;
        while (timeNow / transportTime <= 1)
        {
            timeNow += Time.deltaTime;
            transform.position = Vector3.Lerp(targetTransform.position, startPos, Mathf.SmoothStep(-1, 1, timeNow / transportTime));
            transform.rotation = Quaternion.Lerp(targetTransform.rotation, startRot, Mathf.SmoothStep(-1, 1, timeNow / transportTime));
            yield return null;
        }

        m_IsDragging = false;
        OnDraggedEnd.Invoke();
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
        float smoothDist = Mathf.Lerp(curDist, targetDist, Mathf.Min(20 * Time.deltaTime, 1));
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