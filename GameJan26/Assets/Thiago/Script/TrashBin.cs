using UnityEngine;

public class TrashBin : MonoBehaviour
{
    [Header("Área da Lixeira")]
    public Collider trashZone;

    public bool IsInside(Vector3 position)
    {
        if (trashZone == null)
            return false;

        return trashZone.bounds.Contains(position);
    }
}