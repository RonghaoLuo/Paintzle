using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Can pool any GameObject
/// </summary>
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [System.Serializable]
    private struct PoolDefinition 
    { 
        public PoolableType type; 
        public GameObject prefab; 
        public int initialSize;
        [System.NonSerialized]
        public Transform poolParent;
    }

    private Dictionary<PoolableType, SimplePool> poolMap = 
        new Dictionary<PoolableType, SimplePool>();

    [SerializeField] private PoolDefinition[] definedPools;
    [SerializeField] private PoolDefinition[] postStartPools;

    private bool isPostStartPoolsInitialized = false;

    #region Reference Maps
    public Dictionary<GameObject, Paintball> gameObjectToPaintballMap = new();

    #endregion

    void Awake()
    {
        //Debug.Log("Pool Manager Woke");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        { 
            Destroy(gameObject); 
            return; 
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnInitializePostStart -= InitializePostStartPools;
    }

    private void Start()
    {
        GameManager.Instance.OnInitializePostStart += InitializePostStartPools;
        InitializeDefinedPools();
    }

    private void InitializeDefinedPools()
    {
        for (int i = 0; i < definedPools.Length; i++)
        {
            PoolDefinition def = definedPools[i];

            GameObject poolParent = new(def.type.ToString());
            poolParent.transform.SetParent(this.transform, false);
            poolMap[def.type] = new SimplePool(def.prefab, poolParent.transform, def.initialSize);
        }
    }

    private void InitializePostStartPools()
    {
        if (isPostStartPoolsInitialized)
            return;
        for (int i = 0; i < postStartPools.Length; i++)
        {
            PoolDefinition def = postStartPools[i];
            GameObject poolParent = new(def.type.ToString());
            poolParent.transform.SetParent(this.transform, false);
            poolMap[def.type] = new SimplePool(def.prefab, poolParent.transform, def.initialSize);
        }
        isPostStartPoolsInitialized = true;
    }

    public GameObject Spawn(PoolableType type, Vector3 pos, Quaternion rot, Vector3 velocity)
    {
        if (!poolMap.TryGetValue(type, out var pool))
        {
            Debug.LogError($"[PoolManager] No pool for {type}");
            return null;
        }
        //Debug.Log("before retrieving a gameobject from a pool");
        GameObject go = pool.Get();
        //Debug.Log("retrieved a gameobject from a pool");

        go.transform.position = pos;
        go.transform.rotation = rot;
        //Debug.Log("finished setting position and rotation of a poolable");

        if (go.TryGetComponent<Rigidbody>(out var rb)) 
            rb.linearVelocity = velocity;

        return go;
    }

    public GameObject Spawn(PoolableType type, Transform shootOrigin, float shootSpeed)
    {
        return Spawn(type, shootOrigin.transform.position, 
            shootOrigin.transform.rotation, 
            shootOrigin.forward * shootSpeed);
    }

    public GameObject Spawn(PoolableType type)
    {
        if (!poolMap.TryGetValue(type, out var pool))
        {
            Debug.LogError($"[PoolManager] No pool for {type}");
            return null;
        }
        //Debug.Log("before retrieving a gameobject from a pool");
        GameObject go = pool.Get();
        //Debug.Log("retrieved a gameobject from a pool");

        return go;
    }

    public void ReturnToPool(IPoolable poolable)
    {
        if (!poolMap.TryGetValue(poolable.Type, out var pool))
        {
            Debug.LogWarning($"[PoolManager] No pool for {poolable.Type}, destroying.");
            Destroy(poolable.GameObject);
            return;
        }

        pool.Return(poolable);
    }
}


public enum PoolableType
{
    Ball, Paintball, Bullet, Enemy, Prey, VFX_PaintImpactBurst
}