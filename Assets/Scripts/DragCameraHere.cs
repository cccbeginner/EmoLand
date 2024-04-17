using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DragCameraHere : MonoBehaviour
{
    public float TimeStay = 10f;
    public void DragCameraHereForWhile()
    {
        ThirdPersonCamera.main.DragCameraToTransform(transform, TimeStay);
    }
}
