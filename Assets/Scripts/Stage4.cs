using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stage4 : MonoBehaviour
{
    [SerializeField]
    Flower Flower1, Flower2, Flower3;
    [SerializeField]
    GameObject ToggleButton1, ToggleButton2, ToggleButton3;

    public UnityEvent OnSuccess;
    public bool success { get; private set; }

    private void Start()
    {
        success = false; 
        DisableButtons();
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
        if (success) return;
        Flower1.Toggle(0);
        CheckSuccess();
    }

    public void OnFlower2Press()
    {
        if (success) return;
        Flower1.Toggle(0);
        Flower2.Toggle(0);
        CheckSuccess();
    }

    public void OnFlower3Press()
    {
        if (success) return;
        Flower1.Toggle(0);
        Flower3.Toggle(0);
        CheckSuccess();
    }

    public void EnableButtons()
    {
        Debug.Log("Enable");
        ToggleButton1.SetActive(true);
        ToggleButton2.SetActive(true);
        ToggleButton3.SetActive(true);
    }
    private void DisableButtons()
    {
        ToggleButton1.SetActive(false);
        ToggleButton2.SetActive(false);
        ToggleButton3.SetActive(false);
    }
}
