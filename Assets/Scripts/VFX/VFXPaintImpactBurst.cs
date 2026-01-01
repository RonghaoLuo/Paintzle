using UnityEngine;

public class VFXPaintImpactBurst : VFX
{
    public override PoolableType Type => PoolableType.VFX_PaintImpactBurst;
    public override GameObject GameObject => gameObject;

    protected ParticleSystem splashCard, droplets;

    // Particle System Modules for easy access;
    // Interfaces that link to Particle System properties
    protected ParticleSystem.MainModule splashCardMain, dropletsMain;
    protected ParticleSystem.EmissionModule dropletsEmission;
    protected ParticleSystem.Burst dropletsBurst;
    protected ParticleSystem.ShapeModule shape;

    protected void Awake()
    {
        splashCard = myParticleSystems[0];
        droplets = myParticleSystems[1];
        splashCardMain = splashCard.main;
        dropletsMain = droplets.main;
        dropletsEmission = droplets.emission;
        dropletsBurst = dropletsEmission.GetBurst(0);
        shape = droplets.shape;
    }

    public override void OnPoolInitialize()
    {
        VFXManager.Instance.RegisterVFX(gameObject, this);
    }
    public override void OnDespawn()
    {
        gameObject.SetActive(false);
    }
    public override void OnSpawn()
    {
        gameObject.SetActive(true);
        despawnTimer = 0f;
        Play();
    }
    public virtual void SetColour(Color colour)
    {
        splashCardMain.startColor = colour;
        dropletsMain.startColor = colour;
    }

    public virtual void SetScales(float cardMultiplier, float dropletMultiplier)
    {
        splashCardMain.startSizeMultiplier = cardMultiplier;
        //dropletsMain.startSizeMultiplier = dropletMultiplier;
    }

    public virtual void SetSpreadRadius(float radius) 
    { 
        //shape.scale = new Vector3(radius, radius, radius); 
    }

    public virtual void SetDropletsBurstCount(int count) 
    {
        dropletsBurst.count = count;
        dropletsEmission.SetBurst(0, dropletsBurst);
    }

    public virtual void SetDropletsSpreadSpeed(float curveMultiplier)
    {
        dropletsMain.startSpeedMultiplier = curveMultiplier;
    }

    protected override void Update()
    {
        despawnTimer += Time.deltaTime;
        if (despawnTimer >= despawnDelay)
        {
            VFXManager.Instance.despawnVFX(this);
        }
    }
    protected override void Play()
    {
        if (droplets != null)
        {
            droplets.Play();
        }
        if (splashCard != null)
        {
            splashCard.Play();
        }
    }
}
