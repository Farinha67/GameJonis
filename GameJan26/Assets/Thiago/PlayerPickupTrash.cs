using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickupTrash : MonoBehaviour
{
    [Header("Holding Point")]
    public Transform holdingPoint;

    [Header("Pickup")]
    public float pickupDistance = 3f;

    private GameObject heldTrash;

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (heldTrash == null)
            {
                TryPickupTrash();
            }
            else
            {
                DropTrash();
            }
        }
    }

    void TryPickupTrash()
    {
        Ray ray = new Ray(
            Camera.main.transform.position,
            Camera.main.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            // Só pode pegar objetos com a tag Trash
            if (hit.collider.CompareTag("Trash"))
            {
                heldTrash = hit.collider.gameObject;

                Rigidbody rb = heldTrash.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                Collider col = heldTrash.GetComponent<Collider>();

                if (col != null)
                {
                    col.enabled = false;
                }

                heldTrash.transform.SetParent(holdingPoint);

                heldTrash.transform.localPosition = Vector3.zero;
                heldTrash.transform.localRotation = Quaternion.identity;
            }
        }
    }

    void DropTrash()
    {
        if (heldTrash == null)
            return;

        // Procura todas as lixeiras
        TrashBin[] trashBins = FindObjectsOfType<TrashBin>();

        foreach (TrashBin trashBin in trashBins)
        {
            if (trashBin.IsInside(heldTrash.transform.position))
            {
                Debug.Log("Lixo jogado na lixeira!");

                Destroy(heldTrash);

                heldTrash = null;

                return;
            }
        }

        // Se não estiver na lixeira, solta normalmente
        heldTrash.transform.SetParent(null);

        heldTrash.transform.position =
            Camera.main.transform.position +
            Camera.main.transform.forward * 1.5f;

        Rigidbody rb = heldTrash.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Collider col = heldTrash.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = true;
        }

        heldTrash = null;
    }
}