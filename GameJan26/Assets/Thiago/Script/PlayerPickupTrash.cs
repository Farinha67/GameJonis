using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickupTrash : MonoBehaviour
{
    [Header("Holding Points")]
    public Transform leftHoldingPoint;
    public Transform rightHoldingPoint;

    [Header("Pickup")]
    public float pickupDistance = 3f;

    private GameObject leftTrash;
    private GameObject rightTrash;

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // G = lixo
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            HandlePickupDrop();
        }
    }

    private void HandlePickupDrop()
    {
        Camera cam =
            Camera.main;

        if (cam == null)
            return;

        Ray ray =
            new Ray(
                cam.transform.position,
                cam.transform.forward
            );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            pickupDistance,
            ~0,
            QueryTriggerInteraction.Collide))
        {
            GameObject trash =
                EncontrarLixo(
                    hit.collider
                );

            if (trash != null)
            {
                if (trash == leftTrash ||
                    trash == rightTrash)
                    return;

                if (leftTrash == null)
                {
                    leftTrash =
                        trash;

                    AttachTrash(
                        leftTrash,
                        leftHoldingPoint
                    );

                    Debug.Log(
                        "🗑️ Lixo na mão esquerda."
                    );

                    return;
                }

                if (rightTrash == null)
                {
                    rightTrash =
                        trash;

                    AttachTrash(
                        rightTrash,
                        rightHoldingPoint
                    );

                    Debug.Log(
                        "🗑️ Lixo na mão direita."
                    );

                    return;
                }

                Debug.Log(
                    "❌ As duas mãos estão ocupadas."
                );

                return;
            }
        }

        DropAvailableTrash();
    }

    private GameObject EncontrarLixo(
        Collider collider)
    {
        if (collider == null)
            return null;

        if (collider.CompareTag("Trash"))
            return collider.gameObject;

        Transform atual =
            collider.transform;

        while (atual != null)
        {
            if (atual.CompareTag("Trash"))
                return atual.gameObject;

            atual =
                atual.parent;
        }

        return null;
    }

    private void AttachTrash(
        GameObject trash,
        Transform holdingPoint)
    {
        if (trash == null ||
            holdingPoint == null)
            return;

        Rigidbody rb =
            trash.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider[] cols =
            trash.GetComponentsInChildren<Collider>();

        foreach (Collider col in cols)
        {
            col.enabled = false;
        }

        trash.transform.SetParent(
            holdingPoint
        );

        trash.transform.localPosition =
            Vector3.zero;

        trash.transform.localRotation =
            Quaternion.identity;
    }

    private void DropAvailableTrash()
    {
        if (rightTrash != null)
        {
            DropTrash(
                ref rightTrash
            );

            return;
        }

        if (leftTrash != null)
        {
            DropTrash(
                ref leftTrash
            );

            return;
        }

        Debug.Log(
            "❌ Você não está segurando nenhum lixo."
        );
    }

    private void DropTrash(
        ref GameObject trash)
    {
        if (trash == null)
            return;

        TrashBin[] bins =
            FindObjectsByType<TrashBin>(
                FindObjectsSortMode.None
            );

        foreach (TrashBin bin in bins)
        {
            if (bin == null)
                continue;

            if (bin.IsInside(
                trash.transform.position))
            {
                Destroy(trash);

                trash = null;

                Debug.Log(
                    "🗑️ Lixo colocado na lixeira!"
                );

                return;
            }
        }

        Camera cam =
            Camera.main;

        Vector3 dropPosition;

        if (cam != null)
        {
            dropPosition =
                cam.transform.position +
                cam.transform.forward *
                1.5f;
        }
        else
        {
            dropPosition =
                transform.position +
                transform.forward *
                1.5f;
        }

        trash.transform.SetParent(null);

        trash.transform.position =
            dropPosition;

        Rigidbody rb =
            trash.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Collider[] cols =
            trash.GetComponentsInChildren<Collider>();

        foreach (Collider col in cols)
        {
            col.enabled = true;
        }

        trash = null;
    }
}