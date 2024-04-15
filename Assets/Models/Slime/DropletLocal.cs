using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DropletLocal : MonoBehaviour
{
    // A static droplet can only be eaten

    public Vector3 UnitSizeScale;//(1,1,1)
    public float UnitSizeRadius = 0.25f;
    public int Size;
    public UnityEvent<int> OnEaten;
    public Rigidbody rigidBody { get; private set; }
    public SphereCollider sphereCollider { get; private set; }


    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        ReloadSize();
    }
    private void ReloadSize()
    {
        float sqrt3 = Mathf.Pow(Size, 1f / 3f);
        transform.localScale = sqrt3 * UnitSizeScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null && ReferenceEquals(player, Player.main))
        {
            DropletNetwork playerDroplet = collision.gameObject.GetComponent<DropletNetwork>();
            playerDroplet.EatDroplet(this);
            BeEatenByDroplet(playerDroplet);
        }
    }

    private void BeEatenByDroplet(DropletNetwork another)
    {
        sphereCollider.enabled = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        StartCoroutine(DelayEaten(another));
    }

    IEnumerator DelayEaten(DropletNetwork another)
    {
        Vector3 targetPos = Vector3.Lerp(another.transform.position, transform.position, Size / (another.size + Size));
        while (true)
        {
            Vector3 distVec = targetPos - transform.position;
            Vector3 moveVec = distVec.normalized * Time.deltaTime * 5 / distVec.magnitude;
            if (moveVec.sqrMagnitude <= distVec.sqrMagnitude)
            {
                transform.position += moveVec;
            }
            else
            {
                transform.position += distVec;
            }
            if (distVec.sqrMagnitude < 0.1)
            {
                break;
            }
            yield return null;
        }
        OnEaten.Invoke(Size);
        Destroy(gameObject);
    }
}
