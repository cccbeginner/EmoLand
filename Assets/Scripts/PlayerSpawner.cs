using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    public GameObject PlayerStart;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Vector3 position = PlayerStart ? PlayerStart.transform.position : new Vector3(10, 1, 10);
            Quaternion rotation = PlayerStart ? PlayerStart.transform.rotation : Quaternion.identity;
            Debug.Log($"{PlayerStart == null} {position} {rotation}");
            Runner.Spawn(PlayerPrefab, position, rotation, player);
        }
    }
}