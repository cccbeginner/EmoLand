using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    public GameObject PlayerStart;
    public UnityEvent OnMainPlayerJoined;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Vector3 position = PlayerStart ? PlayerStart.transform.position : new Vector3(10, 1, 10);
            Quaternion rotation = PlayerStart ? PlayerStart.transform.rotation : Quaternion.identity;
            Runner.Spawn(PlayerPrefab, position, rotation, player);
            OnMainPlayerJoined.Invoke();
        }
    }
}