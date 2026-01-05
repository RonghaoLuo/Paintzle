using UnityEngine;

public class CollectiblePaint : MonoBehaviour
{
    [SerializeField] private Color paintColor = Color.gray5;
    [SerializeField] private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer.material.color = paintColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        CollectionManager.Instance.PlayerPaintInventory.CollectPaint(paintColor);
        AudioManager.Instance.PlaySfx("SFX_Pickup", transform.position);
        Destroy(gameObject);
    }
}
