using UnityEngine;

public class PowerUp : MonoBehaviour, ICollectible
{
    [SerializeField] private float jumpForceMultiplier = 3f;
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float sizeMultiplier = 3f;
    [SerializeField] private float effectRadiusMultiplier = 3f;
    [SerializeField] private float cooldownMultiplier = 0.25f;
    [SerializeField] private float duration = 3f;
    [SerializeField] private bool shootRainbow = true;

    public void CollectTo(PlayerCollect playerCollect)
    {
        //Debug.Log("Getting Collected");
        playerCollect.StartPowerUP(jumpForceMultiplier, speedMultiplier, sizeMultiplier, effectRadiusMultiplier, 
            cooldownMultiplier, duration, shootRainbow);
        CollectionManager.Instance.DespawnPowerUp(this);
        AudioManager.Instance.PlaySfx("SFX_Pickup_Powerup", transform.position);
    }
}
