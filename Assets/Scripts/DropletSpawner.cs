using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropletSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject m_DropletPrefab;

    void Start()
    {
        AddPlayerListener();
    }

    public void AddPlayerListener()
    {
        Player.main.playerJump.OnJumpBegin.AddListener(OnPlayerJump);
        Player.main.playerSprint.OnSprintBegin.AddListener(OnPlayerSprint);
    }


    void SpawnDroplet(Vector3 pos)
    {
        var newDroplet = Instantiate(m_DropletPrefab, pos, Quaternion.identity);

        //newDroplet.GetComponent<Rigidbody>().AddForce(Vector3.down * 10, ForceMode.Impulse);
        newDroplet.GetComponent<DropletLocal>().isEatable = false;
        newDroplet.GetComponent<DropletLocal>().Size = 1;
        StartCoroutine(EatableAfterSec(newDroplet.GetComponent<DropletLocal>(), 0.1f));
    }

    void OnPlayerJump()
    {
        if (!Player.main.droplet.isGrounded && Physics.Raycast(Player.main.transform.position + 0.1f * Vector3.up, -Vector3.up, 0.6f) == false)
        {
            // apply offset to avoid collide with player
            Vector3 offset = Vector3.down * 0.5f;
            Vector3 spawnPos = Player.main.transform.position + offset;
            SpawnDroplet(spawnPos);

            Player.main.droplet.size -= 1;
        }
    }
    void OnPlayerSprint()
    {
        // apply offset to avoid collide with player
        Vector3 offset = -Player.main.transform.forward * 0.5f;
        Vector3 spawnPos = Player.main.transform.position + offset;
        SpawnDroplet(spawnPos);
        //newDroplet.GetComponent<Rigidbody>().AddForce(-Player.main.transform.forward * 10, ForceMode.Impulse);

        Player.main.droplet.size -= 1;
    }

    IEnumerator EatableAfterSec(DropletLocal newDroplet, float delaySec)
    {
        yield return new WaitForSeconds(delaySec);
        newDroplet.isEatable = true;
    }
}
