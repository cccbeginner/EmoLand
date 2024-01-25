using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject : MonoBehaviour
{
    private SceneManager _sceneManager { get; set; }
    private PlayerController _playerController { get; set; }
    void Awake()
    {
        _sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        _playerController = GetComponent<PlayerController>();
    }

    public SceneManager GetSceneManager() { return _sceneManager; }

    private void OnEnable()
    {
        if (_playerController != null)
        {
            _sceneManager.AddPlayer(this, _playerController);
        }
    }

    private void OnDisable()
    {
        if (_playerController != null)
        {
            _sceneManager.RemovePlayer(this, _playerController);
        }
    }
}
