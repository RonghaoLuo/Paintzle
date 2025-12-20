using UnityEngine;

public class Paintball : Projectile
{
    [SerializeField] private Color paintColor = Color.gray5;
    [SerializeField] private float effectRadius;
    [SerializeField] private LayerMask paintableMask;
    [SerializeField] private MeshRenderer meshRenderer;

    protected override void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Paintball Hit");
        base.OnCollisionEnter(collision);

        Vector3 overlapSpherePosition = transform.position;

        PoolManager.Instance.ReturnToPool(this);

        Collider[] paintableColliders = Physics.OverlapSphere(overlapSpherePosition, effectRadius, paintableMask);

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

    public override void InitializePoolable()
    {
        base.InitializePoolable();
        PoolManager.Instance.gameObjectToPaintballMap.Add(GameObject, this);
    }

    public void SetColour(Color colour)
    {
        paintColor = colour;
        meshRenderer.material.color = colour;
    }

    public void SetSize(float size)
    {
        transform.localScale = new(size, size, size);
    }

    public void SetEffectRadius(float effectRadius)
    {
        this.effectRadius = effectRadius;
    }
}
