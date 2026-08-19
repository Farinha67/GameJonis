using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickupTrash : MonoBehaviour
{
    [Header("Holding Points")]
    public Transform leftHoldingPoint;
    public Transform rightHoldingPoint;

    [Header("Pickup")]
    public float pickupDistance = 3f;

    [Header("Sons do Lixo")]
    public AudioSource audioSource;

    // Som quando pega o lixo
    public AudioClip somColeta;

    // Som diferente quando solta no chão
    public AudioClip somSoltar;

    // Som diferente quando coloca na lixeira
    public AudioClip somLixeira;

    private GameObject leftTrash;
    private GameObject rightTrash;

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // G = pegar / soltar lixo
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            HandlePickupDrop();
        }
    }

    private void HandlePickupDrop()
    {
        Camera cam = Camera.main;

        if (cam == null)
            return;

        Ray ray = new Ray(
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
                EncontrarLixo(hit.collider);

            if (trash != null)
            {
                // Não pode pegar o mesmo lixo duas vezes
                if (trash == leftTrash ||
                    trash == rightTrash)
                {
                    return;
                }

                // =========================================
                // MÃO ESQUERDA
                // =========================================

                if (leftTrash == null)
                {
                    leftTrash = trash;

                    AttachTrash(
                        leftTrash,
                        leftHoldingPoint
                    );

                    // 🔊 SOM DE COLETA
                    TocarSomColeta();

                    Debug.Log(
                        "🗑️ Lixo na mão esquerda."
                    );

                    return;
                }

                // =========================================
                // MÃO DIREITA
                // =========================================

                if (rightTrash == null)
                {
                    rightTrash = trash;

                    AttachTrash(
                        rightTrash,
                        rightHoldingPoint
                    );

                    // 🔊 SOM DE COLETA
                    TocarSomColeta();

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

        // Se não encontrou lixo,
        // tenta soltar o que está segurando
        DropAvailableTrash();
    }

    // =====================================================
    // SOM DE COLETA
    // =====================================================

    private void TocarSomColeta()
    {
        if (audioSource == null)
            return;

        if (somColeta == null)
            return;

        audioSource.PlayOneShot(
            somColeta
        );
    }

    // =====================================================
    // SOM DE SOLTAR NO CHÃO
    // =====================================================

    private void TocarSomSoltar()
    {
        if (audioSource == null)
            return;

        if (somSoltar == null)
            return;

        audioSource.PlayOneShot(
            somSoltar
        );
    }

    // =====================================================
    // SOM DA LIXEIRA
    // =====================================================

    private void TocarSomLixeira()
    {
        if (audioSource == null)
            return;

        if (somLixeira == null)
            return;

        audioSource.PlayOneShot(
            somLixeira
        );
    }

    // =====================================================
    // ENCONTRAR LIXO
    // =====================================================

    private GameObject EncontrarLixo(
        Collider collider)
    {
        if (collider == null)
            return null;

        // Verifica o próprio objeto
        if (collider.CompareTag("Trash"))
            return collider.gameObject;

        // Verifica os pais
        Transform atual =
            collider.transform;

        while (atual != null)
        {
            if (atual.CompareTag("Trash"))
                return atual.gameObject;

            atual = atual.parent;
        }

        return null;
    }

    // =====================================================
    // COLOCAR LIXO NA MÃO
    // =====================================================

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

            rb.linearVelocity =
                Vector3.zero;

            rb.angularVelocity =
                Vector3.zero;
        }

        // Desativa os colliders
        Collider[] cols =
            trash.GetComponentsInChildren<Collider>();

        foreach (Collider col in cols)
        {
            col.enabled = false;
        }

        // Coloca na mão
        trash.transform.SetParent(
            holdingPoint
        );

        trash.transform.localPosition =
            Vector3.zero;

        trash.transform.localRotation =
            Quaternion.identity;
    }

    // =====================================================
    // SOLTAR LIXO
    // =====================================================

    private void DropAvailableTrash()
    {
        // Primeiro tenta soltar da mão direita
        if (rightTrash != null)
        {
            DropTrash(
                ref rightTrash
            );

            return;
        }

        // Depois tenta soltar da mão esquerda
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

    // =====================================================
    // SOLTAR UM LIXO
    // =====================================================

    private void DropTrash(
        ref GameObject trash)
    {
        if (trash == null)
            return;

        // =========================================
        // VERIFICA SE ESTÁ EM UMA LIXEIRA
        // =========================================

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
                // 🔊 SOM DIFERENTE DA LIXEIRA
                TocarSomLixeira();

                Destroy(trash);

                trash = null;

                Debug.Log(
                    "🗑️ Lixo colocado na lixeira!"
                );

                return;
            }
        }

        // =========================================
        // POSIÇÃO PARA SOLTAR NO CHÃO
        // =========================================

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

        // =========================================
        // TIRA DA MÃO
        // =========================================

        trash.transform.SetParent(null);

        trash.transform.position =
            dropPosition;

        // =========================================
        // REATIVA RIGIDBODY
        // =========================================

        Rigidbody rb =
            trash.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // =========================================
        // REATIVA COLLIDERS
        // =========================================

        Collider[] cols =
            trash.GetComponentsInChildren<Collider>();

        foreach (Collider col in cols)
        {
            col.enabled = true;
        }

        // =========================================
        // 🔊 SOM DE SOLTAR
        // =========================================

        TocarSomSoltar();

        Debug.Log(
            "🗑️ Lixo solto no chão!"
        );

        trash = null;
    }
}