using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    public GameObject PlayerStart;
    public Vector3 StartVelocity;
    public Vector3 StartCamRotation;
    public UnityEvent OnMainPlayerJoined;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Vector3 position = PlayerStart ? PlayerStart.transform.position : new Vector3(10, 1, 10);
            Quaternion rotation = PlayerStart ? PlayerStart.transform.rotation : Quaternion.identity;
            Runner.Spawn(PlayerPrefab, position, rotation, player);
            OnMainPlayerJoined.Invoke();
            SpawnPlayerFromWaterFall();
        }
    }

    private void SpawnPlayerFromWaterFall()
    {
        // Player starts from the bottom of waterfall,
        //  and then splash out.
        // Actually just add a splash force to player.
        Player.main.rigidBody.AddForce(StartVelocity, ForceMode.VelocityChange);
        ThirdPersonCamera.main.SetRotation(StartCamRotation);
    }
}