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

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            HandlePickupDrop();
        }
    }

    void HandlePickupDrop()
    {
        Ray ray = new Ray(
            Camera.main.transform.position,
            Camera.main.transform.forward
        );

        // Primeiro verifica o que o jogador está mirando
        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            // Está mirando em um lixo
            if (hit.collider.CompareTag("Trash"))
            {
                GameObject trash = hit.collider.gameObject;

                // Não deixa pegar o mesmo lixo duas vezes
                if (trash == leftTrash || trash == rightTrash)
                    return;

                // Coloca na primeira mão disponível
                if (leftTrash == null)
                {
                    leftTrash = trash;
                    AttachTrash(leftTrash, leftHoldingPoint);
                    return;
                }

                if (rightTrash == null)
                {
                    rightTrash = trash;
                    AttachTrash(rightTrash, rightHoldingPoint);
                    return;
                }

                // As duas mãos estão ocupadas
                return;
            }
        }

        // Não está mirando em lixo → solta
        DropAvailableTrash();
    }

    void AttachTrash(GameObject trash, Transform holdingPoint)
    {
        Rigidbody rb = trash.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = trash.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }

        trash.transform.SetParent(holdingPoint);

        trash.transform.localPosition = Vector3.zero;
        trash.transform.localRotation = Quaternion.identity;
    }

    void DropAvailableTrash()
    {
        // Se tiver lixo na mão direita, solta ele primeiro
        if (rightTrash != null)
        {
            DropTrash(ref rightTrash);
            return;
        }

        // Senão, solta o da mão esquerda
        if (leftTrash != null)
        {
            DropTrash(ref leftTrash);
            return;
        }
    }

    void DropTrash(ref GameObject trash)
    {
        if (trash == null)
            return;

        // Verifica se o lixo está dentro de alguma lixeira
        TrashBin[] trashBins = FindObjectsOfType<TrashBin>();

        foreach (TrashBin trashBin in trashBins)
        {
            if (trashBin.IsInside(trash.transform.position))
            {
                Debug.Log("Lixo jogado na lixeira!");

                Destroy(trash);
                trash = null;

                return;
            }
        }

        // Posição para soltar
        Vector3 dropPosition =
            Camera.main.transform.position +
            Camera.main.transform.forward * 0.5f;

        // Tira da mão
        trash.transform.SetParent(null);

        trash.transform.position = dropPosition;

        // Reativa física
        Rigidbody rb = trash.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Reativa colisão
        Collider col = trash.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = true;
        }

        trash = null;
    }
}