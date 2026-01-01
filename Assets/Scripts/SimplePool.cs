using System.Collections.Generic;
using UnityEngine;

public class SimplePool
{
    private readonly struct PooledObject
    {
        public readonly GameObject gameObject;
        public readonly IPoolable poolable;
        public PooledObject(GameObject gameObject, IPoolable poolable)
        {
            this.gameObject = gameObject;
            this.poolable = poolable;
        }
    }

    // All assigned in constructor
    private readonly GameObject prefab;
    private readonly Transform poolParent;
    private readonly Queue<PooledObject> queue = new Queue<PooledObject>();

    public SimplePool(GameObject prefab, Transform parent = null, int initialSize = 10)
    {
        this.prefab = prefab;
        this.poolParent = parent;
        InitializePool(Mathf.Max(1, initialSize));
    }

    private void InitializePool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = Object.Instantiate(prefab, poolParent);
            IPoolable p = go.GetComponent<IPoolable>();
            if (p == null)
            {
                Debug.LogError($"[SimplePool] Object '{prefab.name}' does not implement IPoolable.");
                return;
            }
            p.OnPoolInitialize();
            go.SetActive(false);
            queue.Enqueue(new PooledObject(go, p));
        }
    }

    public GameObject Get()
    {
        PooledObject po;
        if (queue.Count > 0)
        {
            po = queue.Dequeue();
        }
        else
        {
            po = CreateNewPooledObject();
        }
        po.poolable.OnSpawn();
        return po.gameObject;
    }

    private PooledObject CreateNewPooledObject()
    {
        GameObject go = Object.Instantiate(prefab, poolParent);
        IPoolable po = go.GetComponent<IPoolable>();
        po.OnPoolInitialize();

        return new PooledObject(go, po);
    }

    /// <summary>
    /// Uses one GetComponent call
    /// </summary>
    /// <param name="go"></param>
    //public void Return(GameObject go)
    //{
    //    IPoolable poolable = go.GetComponent<IPoolable>();
    //    poolable.OnDespawn();
    //    queue.Enqueue(new PooledObject { gameObject = go, poolable = poolable });
    //}

    public void Return(IPoolable p)
    {
        p.OnDespawn();
        queue.Enqueue(new PooledObject(p.GameObject, p));
    }
}
