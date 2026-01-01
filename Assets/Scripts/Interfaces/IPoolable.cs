using UnityEngine;

public interface IPoolable
{
    PoolableType Type { get; }
    GameObject GameObject { get; }
    void OnSpawn();
    void OnDespawn();
    void OnPoolInitialize();
}
