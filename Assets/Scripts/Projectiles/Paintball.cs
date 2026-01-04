using UnityEngine;

public class Paintball : Projectile
{
    [SerializeField] private Color paintColor = Color.gray5;
    [SerializeField] private float effectRadius;
    [SerializeField] private LayerMask paintableMask;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Transform ballBody;
    [SerializeField] private TrailRenderer trail;

    [Header("VFX Properties")]
    [SerializeField] private float sizeMultiplier = 1f;
    [SerializeField] private float countMultiplier = 1f;
    [SerializeField] private float spreadSpeedMultiplier = 1f;
    [SerializeField] private float baseBallScale = 0.19f;
    [SerializeField] private float baseTrailWidthMultiplier = 0.2f;

    protected override void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Paintball Hit");
        base.OnCollisionEnter(collision);
        VFXManager.Instance.SpawnPaintImpactBurst(transform.position, 
            Quaternion.LookRotation(collision.GetContact(0).normal), paintColor,
            sizeMultiplier, countMultiplier, spreadSpeedMultiplier);

        Vector3 overlapSpherePosition = transform.position;

        PoolManager.Instance.ReturnToPool(this);

        Collider[] paintableColliders = Physics.OverlapSphere(overlapSpherePosition, 
            effectRadius, paintableMask);

        foreach (Collider collider in paintableColliders)
        {
            if (!collider.gameObject.TryGetComponent<Paintable>(out Paintable paintable))
            {
                //Debug.Log("Didn't get a Paintable");
            }
            if (paintable != null)
            {
                paintable.Paint(paintColor);
            }


            if (!collider.gameObject.TryGetComponent<IDefeatable>(out IDefeatable defeatable))
            {
                //Debug.Log("Didn't get a Defeatable");
            }
            if (defeatable != null)
            {
                defeatable.Hit();
            }

        }
    }

    public override void OnPoolInitialize()
    {
        base.OnPoolInitialize();
        PoolManager.Instance.gameObjectToPaintballMap.Add(GameObject, this);
    }

    public void SetColour(Color colour)
    {
        paintColor = colour;
        meshRenderer.material.color = colour;
    }

    public void SetSize(float multiplier)
    {
        //transform.localScale = new(size, size, size);
        ballBody.localScale = new Vector3(baseBallScale * multiplier, 
            baseBallScale * multiplier, baseBallScale * multiplier);
        trail.widthMultiplier = baseTrailWidthMultiplier * multiplier;
    }

    public void SetEffectRadius(float effectRadius)
    {
        this.effectRadius = effectRadius;
    }

    public void SetImpactVFXProperties(float sizeMultiplier, float countMultiplier, float spreadSpeedMultiplier)
    {
        this.sizeMultiplier = sizeMultiplier;
        this.countMultiplier = countMultiplier;
        this.spreadSpeedMultiplier = spreadSpeedMultiplier;
    }
}
