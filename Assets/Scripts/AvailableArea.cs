using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;

public class AvailableArea : MonoBehaviour
{
    public GameObject StartArea;
    public List<TreeArea> TreeAreaList;
    public List<float> AreaRadius;
    public UnityEvent OnPlayerExit;
    private List<Vector2> AreaPoints = new List<Vector2>();
    private void Start()
    {
        AreaPoints.Add(TwoDPos(StartArea.transform.position));
    }

    private void Update()
    {
        CheckUpdateAreaPoints();
        if (IsPlayerExit())
        {
            OnPlayerExit.Invoke();
        }
    }

    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector3(StartArea.transform.position.x, 10, StartArea.transform.position.z), 50);
        for (int i = 0; i < AreaPoints.Count; i++)
        {
            Vector2 p = AreaPoints[i];
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(new Vector3(p.x, 10, p.y), AreaRadius[i]);
        }
    }*/

    private Vector2 TwoDPos(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    private void CheckUpdateAreaPoints()
    {
        for(int i = 0; i < TreeAreaList.Count; i++)
        {
            if (TreeAreaList[i].isOn)
            {
                Vector3 twoDPos = TwoDPos(TreeAreaList[i].transform.position);
                if (!AreaPoints.Contains(twoDPos))
                {
                    AreaPoints.Add(twoDPos);
                }
            }
        }
    }

    private bool IsPlayerExit()
    {
        if (Player.main == null) return false;

        Vector2 playerPos = TwoDPos(Player.main.transform.position);
        bool exit = true;
        for (int i = 0; i < AreaPoints.Count; i++)
        {
            float sqrDist = (AreaPoints[i] - playerPos).sqrMagnitude;
            if (sqrDist <= AreaRadius[i] * AreaRadius[i])
            {
                exit = false;
            }
        }
        return exit;
    }
}
