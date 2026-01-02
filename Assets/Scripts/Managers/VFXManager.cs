using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("VFX Settings")]
    [Header("Default Paint Impact Burst Settings")]
    [SerializeField] private float baseCardScaleMultiplier = 2.2f;
    [SerializeField] private float baseDropletScaleMultiplier = 0.3f;
    [SerializeField] private float baseSpreadSpeedMultiplier = 10f;
    [SerializeField] private int numberOfDroplets = 20;

    private Dictionary<GameObject, VFX> gameObjectToVFXMap = new();
    private Dictionary<VFXType, PoolableType> vfxToPool = new()
    {
        { VFXType.PaintImpactBurst, PoolableType.VFX_PaintImpactBurst }
    };

    void Awake()
    {
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

    private void Start()
    {
        GameManager.Instance.OnResetManagers += ResetManager;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnResetManagers -= ResetManager;
    }

    private void ResetManager()
    {

    }

    private GameObject SpawnVFX(VFXType type, Vector3 position, Quaternion rotation)
    {
        return PoolManager.Instance.Spawn(vfxToPool[type], position, rotation, Vector3.zero);
    }

    public void SpawnPaintImpactBurst(Vector3 position, Quaternion rotation, 
        Color color, float scaleMultiplier = 1, 
        float dropletCountMultiplier = 1, float dropletSpeedMultiplier = 1)
    {
        GameObject go = SpawnVFX(VFXType.PaintImpactBurst, position, rotation);
        VFXPaintImpactBurst vfx = gameObjectToVFXMap[go] as VFXPaintImpactBurst;

        if (vfx == null)
            return;

        vfx.SetColour(color);
        vfx.SetScales(baseCardScaleMultiplier * scaleMultiplier, 
            baseDropletScaleMultiplier * scaleMultiplier);
        vfx.SetSpreadRadius(scaleMultiplier);
        vfx.SetDropletsBurstCount((int) (numberOfDroplets * dropletCountMultiplier));
        vfx.SetDropletsSpreadSpeed(baseSpreadSpeedMultiplier * dropletSpeedMultiplier);
    }

    public void despawnVFX(VFX vfx)
    {
        PoolManager.Instance.ReturnToPool(vfx);
    }

    public void RegisterVFX(GameObject go, VFX vfx)
    {
        gameObjectToVFXMap.Add(go, vfx);
    }
}

public enum VFXType
{
    PaintImpactBurst
}