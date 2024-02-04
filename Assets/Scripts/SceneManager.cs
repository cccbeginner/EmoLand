using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public List<SceneObject> Players { get; private set; }
    public SceneObject MainPlayer { get; private set; }
    public NetworkRunner Runner { get; private set; }
    private void Start()
    {
        // Find correct runner
        foreach (var runner in NetworkRunner.Instances)
        {
            if (runner.IsStarting)
            {
                Runner = runner; break;
            }
        }
    }

    private void Awake()
    {
        Players = new List<SceneObject>();
    }
    private bool CheckPlayerValid(SceneObject sceneObject, PlayerController playerController)
    {
        if (sceneObject == null) { return false; }
        if (playerController == null) { return false; }
        if (sceneObject.gameObject != playerController.gameObject) { return false; }
        return true;
    }
    public void AddPlayer(SceneObject sceneObject, PlayerController playerController)
    {
        // Directly return if the game object is not player
        if (CheckPlayerValid(sceneObject, playerController) == false)
        {
            return;
        }
        // Add the player
        Players.Add(sceneObject);
        if (playerController.HasStateAuthority)
        {
            MainPlayer = sceneObject;
        }
    }

    public void RemovePlayer(SceneObject sceneObject, PlayerController playerController)
    {
        // Directly return if the game object is not player
        if (CheckPlayerValid(sceneObject, playerController) == false)
        {
            return;
        }

        // Add the player
        Players.Remove(sceneObject);
        if (playerController.HasStateAuthority)
        {
            MainPlayer = null;
        }
    }
}
