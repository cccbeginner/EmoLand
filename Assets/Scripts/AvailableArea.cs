using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AvailableArea : MonoBehaviour
{
    public UnityEvent OnPlayerExit;


    private void OnTriggerExit(Collider other)
    {
        if (ReferenceEquals(other.gameObject, Player.main.gameObject))
        {
            OnPlayerExit.Invoke();
        }
    }
}
