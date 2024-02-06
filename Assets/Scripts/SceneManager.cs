using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public List<GameObject> Players { get; private set; }
    public GameObject MainPlayer { get; private set; }
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
        Players = new List<GameObject>();
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
        Players.Add(sceneObject.gameObject);
        if (playerController.HasStateAuthority)
        {
            MainPlayer = sceneObject.gameObject;
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
        Players.Remove(sceneObject.gameObject);
        if (playerController.HasStateAuthority)
        {
            MainPlayer = null;
        }
    }
}
