using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;

public class DropletLocal : MonoBehaviour
{
    // A static droplet can only be eaten

    public Vector3 UnitSizeScale;//(1,1,1)
    public float UnitSizeRadius = 0.25f;
    public int SizeMin = 1;
    public int SizeMax
    {
        get
        {
            return PlayerDataSystem.currentStage + 1;
        }
    }
    private int m_Size = 1;
    public UnityEvent<int> OnResize;

    public int Size
    {
        get
        {
            return m_Size;
        }
        set
        {
            m_Size = value;
            if (m_Size < SizeMin) m_Size = SizeMin;
            if (m_Size > SizeMax) m_Size = SizeMax;
            ReloadSize();
            OnResize.Invoke(m_Size);
        }
    }
    public UnityEvent<int> OnEaten;
    public Rigidbody rigidBody { get; private set; }
    public SphereCollider sphereCollider { get; private set; }

    // droplet can be eaten or eat others only if isEatable == true
    public bool isEatable = false;

    // about lifetime
    public float LifeTimeSec = 60;
    public float TimeSinceCreated { get; private set; }
    private float m_TimeCreated = 0f;


    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        ReloadSize();
        m_TimeCreated = Time.time;
    }
    private void Update()
    {
        // Manage lifetime
        // Should destroy when life is over
        TimeSinceCreated = Time.time - m_TimeCreated;
        if (TimeSinceCreated >= LifeTimeSec)
        {
            // Be eaten by the ground
            OnEaten.Invoke(Size);
            Destroy(gameObject);
        }
    }
    private void ReloadSize()
    {
        float sqrt3 = Mathf.Pow(Size, 1f / 3f);
        transform.localScale = sqrt3 * UnitSizeScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // droplet can be eaten or eat others only if isEatable == true
        if (isEatable)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            DropletLocal dropletLocal = collision.gameObject.GetComponent<DropletLocal>();

            if (player != null /*&& ReferenceEquals(player, Player.main)*/)
            {
                // be eaten by player
                DropletPlayer playerDroplet = collision.gameObject.GetComponent<DropletPlayer>();
                playerDroplet.EatDroplet(this);
                isEatable = false;
                BeEatenByDroplet(transform);
            }
            else if (dropletLocal != null && dropletLocal.isEatable)
            {
                // eat that droplet
                dropletLocal.isEatable = false;
                dropletLocal.BeEatenByDroplet(transform);
                EatDroplet(dropletLocal.Size);
            }
        }
    }
    private void EatDroplet(int sizeAdd)
    {
        Size += sizeAdd;
        EatAnime();
        m_TimeCreated = Time.time;
        TimeSinceCreated = 0;
    }
    public void EatAnime()
    {
        Animator slimeAnimator = GetComponentInChildren<Animator>();
        slimeAnimator.SetTrigger("Eat");
    }

    private void BeEatenByDroplet(Transform anotherDroplet)
    {
        sphereCollider.enabled = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        StartCoroutine(DelayEaten(anotherDroplet));
    }

    IEnumerator DelayEaten(Transform anotherDroplet)
    {
        Vector3 targetPos = anotherDroplet.position;
        while (true)
        {
            Vector3 distVec = targetPos - anotherDroplet.position;
            Vector3 moveVec = distVec.normalized * Time.deltaTime * 5 / distVec.magnitude;
            if (moveVec.sqrMagnitude <= distVec.sqrMagnitude)
            {
                anotherDroplet.position += moveVec;
            }
            else
            {
                anotherDroplet.position += distVec;
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
