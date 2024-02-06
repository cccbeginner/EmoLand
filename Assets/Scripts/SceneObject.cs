using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject : MonoBehaviour
{
    public SceneManager SceneManager { get; private set; }
    void Awake()
    {
        SceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
    }

    public void AddScenePlayer(PlayerController playerController)
    {
        SceneManager.AddPlayer(this, playerController);
    }
    public void RemoveScenePlayer(PlayerController playerController)
    {
        SceneManager.RemovePlayer(this, playerController);
    }
    public List<GameObject> GetPlayers()
    {
        return SceneManager.Players;
    }
    public GameObject GetMainPlayer()
    {
        return SceneManager.MainPlayer;
    }
}
