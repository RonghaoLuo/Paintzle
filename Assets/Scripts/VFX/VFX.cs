using UnityEngine;

public abstract class VFX : MonoBehaviour, IPoolable
{
    [SerializeField] protected ParticleSystem[] myParticleSystems;
    [SerializeField] protected float despawnDelay = 1f;
    [SerializeField] protected float despawnTimer = 0f;

    public virtual PoolableType Type => throw new System.NotImplementedException();

    public virtual GameObject GameObject => throw new System.NotImplementedException();

    public virtual void OnPoolInitialize()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnDespawn()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnSpawn()
    {
        throw new System.NotImplementedException();
    }

    protected virtual void Update()
    {
        throw new System.NotImplementedException();
    }

    protected virtual void Play()
    {
        throw new System.NotImplementedException();
    }
}
