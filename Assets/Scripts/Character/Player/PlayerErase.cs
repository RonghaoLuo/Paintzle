using UnityEngine;

public class PlayerErase : MonoBehaviour
{
    [SerializeField] private Transform origin;

    public void ErasePaint()
    {
        if (!Physics.Raycast(origin.position, origin.forward, out RaycastHit hitInfo, 10f))
        {
            return;
        }
        if (!hitInfo.collider.TryGetComponent<Paintable>(out Paintable paintable))
        {
            return;
        }
        paintable.ErasePaint();
    }
}
