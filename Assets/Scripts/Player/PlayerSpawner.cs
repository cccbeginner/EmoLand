using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public Transform[] PlayerStartByStage;
    public Vector3[] StartCamRotationByStage;
    public Vector3 StartVelocity;
    public UnityEvent OnMainPlayerJoined;
    public float PlayerJoinDelay = 1f;

    void Start()
    {
        StartCoroutine(InvokeJoinAfterSec());
    }

    IEnumerator InvokeJoinAfterSec()
    {
        yield return new WaitForSeconds(PlayerJoinDelay);
        OnMainPlayerJoined.Invoke();
    }

    public void ResetMainPlayerPos()
    {
        int curStage = PlayerDataSystem.currentStage;
        if (curStage < 0 || curStage >= PlayerStartByStage.Length) return;
        Transform playerStart = PlayerStartByStage[curStage];
        Vector3 position = playerStart ? playerStart.position : new Vector3(10, 1, 10);
        Quaternion rotation = playerStart ? playerStart.rotation : Quaternion.identity;
        Player.main.transform.position = position;
        Player.main.transform.rotation = rotation;

        Vector3 camRotation = StartCamRotationByStage[curStage];
        ThirdPersonCamera.main.SetRotation(camRotation);
    }
}