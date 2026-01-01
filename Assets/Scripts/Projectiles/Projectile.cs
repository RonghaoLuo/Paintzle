using UnityEngine;

public abstract class Projectile : MonoBehaviour, IPoolable
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected float maxLifeTime = 2f;
    [SerializeField] protected float timeOfSpawn;
    [SerializeField] protected PoolableType type;

    public PoolableType Type => type;

    public GameObject GameObject => gameObject;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// USE POOL MANAGER
    /// </summary>
    public virtual void OnSpawn()
    {
        gameObject.SetActive(true);
        timeOfSpawn = Time.time;

        if (rb != null) 
        { 
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero; 
            rb.WakeUp(); 
        }
        // reset any visual or owner info here
    }

    /// <summary>
    /// USE POOL MANAGER
    /// </summary>
    public virtual void OnDespawn()
    {
        // Stop particle systems, disable effects
        gameObject.SetActive(false);

        if (rb != null) 
        { 
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero; 
            rb.Sleep(); 
        }
    }

    protected virtual void Update()
    {
        if (Time.time - timeOfSpawn >= maxLifeTime) 
            PoolManager.Instance.ReturnToPool(this);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        // apply damage/effects...
    }

    public virtual void OnPoolInitialize()
    {
        
    }
}

