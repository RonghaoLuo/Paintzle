using UnityEngine;

public class PlayerShootPaintball : MouseClickStrategy
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform weaponTip;
    [SerializeField] private float maxShootDistance = 1000f;
    [SerializeField] private PaintInventory inventory;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private CharacterController playerController;
    [SerializeField] private PoolableType toShoot;

    [Header("Base Stats")]
    [SerializeField] private float baseProjectileSpeed = 10f;
    [SerializeField] private float baseShootCooldown = 0.5f;
    //[SerializeField] private float basePaintballSize = 1f;
    [SerializeField] private float baseEffectRadius = 1f;

    [Header("Multipliers")]
    [SerializeField] private float projectileSpeedMultiplier = 1f;
    [SerializeField] private float shootCooldownMultiplier = 1f;
    [SerializeField] private float paintballSizeMultiplier = 1f;
    [SerializeField] private float effectRadiusMultiplier = 1f;

    [Header("Toggles")]
    [SerializeField] private bool shootWithRandomColour;

    private float CurrentProjectileSpeed => baseProjectileSpeed * projectileSpeedMultiplier;
    private float CurrentShootCooldown => baseShootCooldown * shootCooldownMultiplier;
    //private float CurrentPaintballSize => basePaintballSize * paintballSizeMultiplier;
    private float CurrentEffectRadius => baseEffectRadius * effectRadiusMultiplier;


    private float shootCooldownTimer;
    private float powerUpTimer = 0f;

    private void Update()
    {
        shootCooldownTimer -= Time.deltaTime;

        #region Power Up, Reset Multipliers
        if (powerUpTimer > 0)
        {
            powerUpTimer -= Time.deltaTime;

            if (powerUpTimer <= 0)
            {
                ResetMultipliersAndToggles();
                UIManager.Instance.OnPowerUpEnd?.Invoke();
            }
        }
        #endregion
    }

    private void ResetMultipliersAndToggles()
    {
        projectileSpeedMultiplier = 1f;
        paintballSizeMultiplier = 1f;
        effectRadiusMultiplier = 1f;
        shootCooldownMultiplier = 1f;
        shootWithRandomColour = false;
    }

    public override void ExecuteStrategy()
    {
        if (shootCooldownTimer <= 0f)
        {
            Shoot();
            shootCooldownTimer = CurrentShootCooldown;
        }
    }

    public void Shoot()
    {
        #region Finding Target Point
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, maxShootDistance))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(maxShootDistance); // Point in the distance if no hit

        //Calculate direction from muzzle to target
        Vector3 directionToTarget = (targetPoint - weaponTip.position).normalized;
        #endregion

        GameObject go = PoolManager.Instance.Spawn(toShoot, weaponTip.transform, 
            CurrentProjectileSpeed + Vector3.Dot(playerController.velocity, directionToTarget));
        Paintball paintball = PoolManager.Instance.gameObjectToPaintballMap[go];
        
        if (shootWithRandomColour)
        {
            paintball.SetColour(inventory.RandomPaint);
        }
        else
        {
            paintball.SetColour(inventory.SelectedPaint);
        }

        paintball.SetSize(paintballSizeMultiplier);
        paintball.SetEffectRadius(CurrentEffectRadius);
        paintball.SetImpactVFXProperties(
            sizeMultiplier: paintballSizeMultiplier,
            countMultiplier: paintballSizeMultiplier,
            spreadSpeedMultiplier: projectileSpeedMultiplier
        );

        AudioManager.Instance.PlaySfx("PaintShoot", transform.position);
    }

    public void StartPowerUp(float projectileSpeed, float size, float effectRafius, 
        float shootCooldown, float duration, bool shootRandomColour)
    {
        projectileSpeedMultiplier = projectileSpeed;
        shootCooldownMultiplier = shootCooldown;
        paintballSizeMultiplier = size;
        effectRadiusMultiplier = effectRafius;
        powerUpTimer = duration;
        shootWithRandomColour = shootRandomColour;

        UIManager.Instance.OnPowerUpStart?.Invoke();
    }
}
