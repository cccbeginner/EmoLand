using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeartLeaf : MonoBehaviour
{
    public UnityEvent OnPlayerFirstStep;
    [SerializeField] Animator m_LeafAnimator;
    bool m_HasBeenStepped = false;
    void Start()
    {
        //DontGrow();
        //Grow();
    }

    public void DontGrow()
    {
        m_LeafAnimator.SetTrigger("DontGrow");
    }

    public void Grow()
    {
        m_LeafAnimator.SetTrigger("Grow");
    }

    public void OnPlayerStep()
    {
        if (!m_HasBeenStepped)
        {
            OnPlayerFirstStep.Invoke();
            m_HasBeenStepped = true;
        }
    }
}
