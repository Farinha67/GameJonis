using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public Collider trashZone;

    public bool IsInside(Vector3 position)
    {
        if (trashZone == null)
            return false;

        return trashZone.bounds.Contains(position);
    }
}