using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stage4 : MonoBehaviour
{
    [SerializeField]
    Flower Flower1, Flower2, Flower3;

    public UnityEvent OnSuccess;
    public bool success { get; private set; }

    private void Start()
    {
        success = false;
        CheckSuccess();
    }

    public void CheckSuccess()
    {
        if (success) return;
        if (Flower1.HasGrow && Flower2.HasGrow && Flower3.HasGrow)
        {
            if (Flower1.IsOpen && Flower2.IsOpen && Flower3.IsOpen)
            {
                success = true;
                OnSuccess.Invoke();
            }
        }
    }

    public void OnFlower1Press()
    {
        Debug.Log("press flower 1");
        if (success) return;
        Flower1.Toggle(0);
        CheckSuccess();
    }

    public void OnFlower2Press()
    {
        Debug.Log("press flower 2");
        if (success) return;
        Flower1.Toggle(0);
        Flower2.Toggle(0);
        CheckSuccess();
    }

    public void OnFlower3Press()
    {
        Debug.Log("press flower 3");
        if (success) return;
        Flower1.Toggle(0);
        Flower3.Toggle(0);
        CheckSuccess();
    }
}
