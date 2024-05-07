using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropletSpawner : MonoBehaviour
{
    NetworkRunner m_NetworkRunner;
    WithMainPlayer withMainPlayer;

    [SerializeField]
    GameObject m_DropletPrefab;
    
    void Start()
    {
        withMainPlayer = GetComponent<WithMainPlayer>();
        withMainPlayer.OnMainPlayerJoin.AddListener(AddEventListener);
        withMainPlayer.OnMainPlayerJoin.AddListener(FindNetworkRunner);
    }

    private void AddEventListener()
    {
        Player.main.playerJump.OnJumpBegin.AddListener(OnPlayerJump);
        Player.main.playerSprint.OnSprintBegin.AddListener(OnPlayerSprint);
    }

    private void FindNetworkRunner()
    {
        foreach (NetworkRunner runner in NetworkRunner.Instances)
        {
            if (runner.isActiveAndEnabled && runner.IsPlayer)
            {
                m_NetworkRunner = runner;
            }
        }
    }
    void OnPlayerJump()
    {
        if (Physics.Raycast(Player.main.transform.position + 0.1f * Vector3.up, -Vector3.up, 0.6f) == false)
        {
            var newDroplet = m_NetworkRunner.Spawn(m_DropletPrefab, Player.main.transform.position);
            if (newDroplet != null)
            {
                newDroplet.GetComponent<Rigidbody>().AddForce(Vector3.down * 10, ForceMode.Impulse);
                newDroplet.GetComponent<DropletNetwork>().isEatable = false;
                StartCoroutine(EatableAfterSec(newDroplet.GetComponent<DropletNetwork>(), 0.1f));
                Player.main.droplet.size -= 1;
            }
        }
    }
    void OnPlayerSprint()
    {
        var newDroplet = m_NetworkRunner.Spawn(m_DropletPrefab, Player.main.transform.position);
        if (newDroplet != null)
        {
            newDroplet.GetComponent<Rigidbody>().AddForce(-Player.main.transform.forward * 10, ForceMode.Impulse);
            newDroplet.GetComponent<DropletNetwork>().isEatable = false;
            StartCoroutine(EatableAfterSec(newDroplet.GetComponent<DropletNetwork>(), 0.1f));
            Player.main.droplet.size -= 1;
        }
    }

    IEnumerator EatableAfterSec(DropletNetwork newDroplet, float delaySec)
    {
        yield return new WaitForSeconds(delaySec);
        newDroplet.isEatable = true;
    }
}
