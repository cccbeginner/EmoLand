using UnityEngine;
using UnityEngine.Events;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public Transform[] PlayerStartByStage;
    public Vector3[] StartCamRotationByStage;
    public Vector3 StartVelocity;
    public UnityEvent OnMainPlayerJoined;

    void Start()
    {
        int curStage = PlayerDataSystem.currentStage;
        Transform playerStart = PlayerStartByStage[curStage];
        if (playerStart == null)
        {
            Debug.LogError($"Cannot find where to spawn player in which current stage is {curStage}.");
        }
        if (StartCamRotationByStage.Length <= curStage)
        {
            Debug.LogError($"Cannot find how to rotate camera in which current stage is {curStage}.");
        }

        ResetMainPlayerPos();

        if (curStage == 0)
        {
            SpawnPlayerFromWaterFall();
        }

        OnMainPlayerJoined.Invoke();
    }

    private void SpawnPlayerFromWaterFall()
    {
        // Player starts from the bottom of waterfall,
        //  and then splash out.
        // Actually just add a splash force to player.
        Player.main.rigidBody.AddForce(StartVelocity, ForceMode.VelocityChange);
    }

    public void ResetMainPlayerPos()
    {
        int curStage = PlayerDataSystem.currentStage;
        Transform playerStart = PlayerStartByStage[curStage];
        Vector3 position = playerStart ? playerStart.position : new Vector3(10, 1, 10);
        Quaternion rotation = playerStart ? playerStart.rotation : Quaternion.identity;
        Player.main.transform.position = position;
        Player.main.transform.rotation = rotation;

        Vector3 camRotation = StartCamRotationByStage[curStage];
        ThirdPersonCamera.main.SetRotation(camRotation);
    }
}