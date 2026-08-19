using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickupTrash : MonoBehaviour
{
    // =====================================================
    // HOLDING POINTS
    // =====================================================

    [Header("Holding Points")]
    public Transform leftHoldingPoint;
    public Transform rightHoldingPoint;

    // =====================================================
    // PICKUP
    // =====================================================

    [Header("Pickup")]
    public float pickupDistance = 3f;

    [Header("Camada do Lixo")]
    public LayerMask camadaDoLixo = ~0;

    // =====================================================
    // PROTEÇÃO CONTRA PAREDES
    // =====================================================

    [Header("Proteção contra paredes")]
    public float distanciaVerificacaoParede = 2.5f;

    public float distanciaVoltaParede = 0.8f;

    public float alturaVoltaParede = 1.0f;

    public float forcaVoltaParede = 2f;

    // =====================================================
    // SONS
    // =====================================================

    [Header("Sons do Lixo")]
    public AudioSource audioSource;

    public AudioClip somColeta;
    public AudioClip somSoltar;
    public AudioClip somLixeira;

    // =====================================================
    // LIXOS NAS MÃOS
    // =====================================================

    private GameObject leftTrash;
    private GameObject rightTrash;

    // =====================================================
    // PLAYER MONEY
    // =====================================================

    private PlayerMoney playerMoney;

    // =====================================================
    // TASK MANAGER
    // =====================================================

    private TrashTaskManager taskManager;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        playerMoney =
            GetComponent<PlayerMoney>();

        taskManager =
            FindFirstObjectByType<TrashTaskManager>();

        if (playerMoney == null)
        {
            Debug.LogError(
                "❌ PlayerMoney não encontrado no Player!"
            );
        }

        if (taskManager == null)
        {
            Debug.LogWarning(
                "⚠️ TrashTaskManager não encontrado!"
            );
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // =================================================
        // E = PEGAR / SOLTAR
        // =================================================

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            HandlePickupDrop();
        }
    }

    // =====================================================
    // PEGAR / SOLTAR
    // =====================================================

    private void HandlePickupDrop()
    {
        Camera cam =
            Camera.main;

        if (cam == null)
        {
            Debug.LogError(
                "❌ Camera.main não encontrada!"
            );

            return;
        }

        // =================================================
        // RAYCAST
        // =================================================

        Ray ray =
            new Ray(
                cam.transform.position,
                cam.transform.forward
            );

        RaycastHit[] hits =
            Physics.RaycastAll(
                ray,
                pickupDistance,
                camadaDoLixo,
                QueryTriggerInteraction.Collide
            );

        // =================================================
        // ORDENAR POR DISTÂNCIA
        // =================================================

        System.Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(
                    b.distance
                )
        );

        // =================================================
        // PROCURAR LIXO
        // =================================================

        foreach (RaycastHit hit in hits)
        {
            GameObject trash =
                EncontrarLixo(
                    hit.collider
                );

            if (trash == null)
                continue;

            // =================================================
            // JÁ ESTÁ NA MÃO
            // =================================================

            if (trash == leftTrash ||
                trash == rightTrash)
            {
                return;
            }

            // =================================================
            // MÃO ESQUERDA
            // =================================================

            if (leftTrash == null)
            {
                leftTrash =
                    trash;

                AttachTrash(
                    leftTrash,
                    leftHoldingPoint
                );

                TocarSomColeta();

                Debug.Log(
                    "🗑️ Lixo na mão esquerda."
                );

                return;
            }

            // =================================================
            // MÃO DIREITA
            // =================================================

            if (rightTrash == null)
            {
                rightTrash =
                    trash;

                AttachTrash(
                    rightTrash,
                    rightHoldingPoint
                );

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

        // =================================================
        // NÃO ESTÁ MIRANDO EM LIXO
        // =================================================

        DropAvailableTrash();
    }

    // =====================================================
    // ENCONTRAR LIXO
    // =====================================================

    private GameObject EncontrarLixo(
        Collider collider)
    {
        if (collider == null)
            return null;

        // =================================================
        // PRÓPRIO OBJETO
        // =================================================

        if (collider.CompareTag("Trash"))
        {
            return collider.gameObject;
        }

        // =================================================
        // PAIS
        // =================================================

        Transform atual =
            collider.transform;

        while (atual != null)
        {
            if (atual.CompareTag("Trash"))
            {
                return atual.gameObject;
            }

            atual =
                atual.parent;
        }

        // =================================================
        // FILHOS
        // =================================================

        Transform[] filhos =
            collider.GetComponentsInChildren<Transform>(
                true
            );

        foreach (Transform filho in filhos)
        {
            if (filho.CompareTag("Trash"))
            {
                return filho.gameObject;
            }
        }

        // =================================================
        // RIGIDBODY PAI
        // =================================================

        Rigidbody rb =
            collider.GetComponentInParent<Rigidbody>();

        if (rb != null)
        {
            if (rb.gameObject.CompareTag("Trash"))
            {
                return rb.gameObject;
            }

            Transform[] rbFilhos =
                rb.GetComponentsInChildren<Transform>(
                    true
                );

            foreach (Transform filho in rbFilhos)
            {
                if (filho.CompareTag("Trash"))
                {
                    return rb.gameObject;
                }
            }
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
        if (trash == null)
            return;

        if (holdingPoint == null)
        {
            Debug.LogError(
                "❌ Holding Point não configurado!"
            );

            return;
        }

        // =================================================
        // GUARDAR TAMANHO REAL
        // =================================================

        Vector3 escalaMundialOriginal =
            trash.transform.lossyScale;

        // =================================================
        // RIGIDBODY
        // =================================================

        Rigidbody rb =
            trash.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb =
                trash.GetComponentInChildren<Rigidbody>();
        }

        if (rb != null)
        {
            rb.isKinematic =
                true;

            rb.useGravity =
                false;

            rb.linearVelocity =
                Vector3.zero;

            rb.angularVelocity =
                Vector3.zero;
        }

        // =================================================
        // DESATIVAR COLLIDERS
        // =================================================

        Collider[] cols =
            trash.GetComponentsInChildren<Collider>(
                true
            );

        foreach (Collider col in cols)
        {
            col.enabled =
                false;
        }

        // =================================================
        // COLOCAR NA MÃO
        // =================================================

        trash.transform.SetParent(
            holdingPoint,
            true
        );

        trash.transform.position =
            holdingPoint.position;

        trash.transform.rotation =
            holdingPoint.rotation;

        // =================================================
        // RESTAURAR TAMANHO MUNDIAL
        // =================================================

        Vector3 escalaPai =
            holdingPoint.lossyScale;

        if (Mathf.Abs(escalaPai.x) > 0.0001f &&
            Mathf.Abs(escalaPai.y) > 0.0001f &&
            Mathf.Abs(escalaPai.z) > 0.0001f)
        {
            trash.transform.localScale =
                new Vector3(
                    escalaMundialOriginal.x /
                    escalaPai.x,

                    escalaMundialOriginal.y /
                    escalaPai.y,

                    escalaMundialOriginal.z /
                    escalaPai.z
                );
        }
    }

    // =====================================================
    // SOLTAR LIXO DISPONÍVEL
    // =====================================================

    private void DropAvailableTrash()
    {
        // =================================================
        // PRIMEIRO DIREITA
        // =================================================

        if (rightTrash != null)
        {
            DropTrash(
                ref rightTrash
            );

            return;
        }

        // =================================================
        // DEPOIS ESQUERDA
        // =================================================

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
    // SOLTAR LIXO
    // =====================================================

    private void DropTrash(
        ref GameObject trash)
    {
        if (trash == null)
            return;

        // =================================================
        // VERIFICAR LIXEIRAS PRIMEIRO
        // =================================================

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
                ColocarNaLixeira(
                    ref trash
                );

                return;
            }
        }

        // =================================================
        // CÂMERA
        // =================================================

        Camera cam =
            Camera.main;

        Vector3 origem;

        Vector3 direcao;

        if (cam != null)
        {
            origem =
                cam.transform.position;

            direcao =
                cam.transform.forward;
        }
        else
        {
            origem =
                transform.position +
                Vector3.up * 1.5f;

            direcao =
                transform.forward;
        }

        // =================================================
        // DETECTAR PAREDE NA FRENTE
        // =================================================

        RaycastHit paredeHit;

        bool bateuEmAlgo =
            Physics.Raycast(
                origem,
                direcao,
                out paredeHit,
                distanciaVerificacaoParede,
                ~0,
                QueryTriggerInteraction.Ignore
            );

        if (bateuEmAlgo)
        {
            // =================================================
            // IGNORAR O PRÓPRIO PLAYER
            // =================================================

            if (paredeHit.collider.transform.IsChildOf(
                transform))
            {
                bateuEmAlgo = false;
            }
        }

        // =================================================
        // VERIFICAR SE É PAREDE
        // =================================================

        if (bateuEmAlgo)
        {
            float verticalidade =
                Mathf.Abs(
                    Vector3.Dot(
                        paredeHit.normal,
                        Vector3.up
                    )
                );

            // =================================================
            // SUPERFÍCIE VERTICAL = PAREDE
            // =================================================

            if (verticalidade < 0.5f)
            {
                VoltarLixoDaParede(
                    ref trash,
                    direcao
                );

                return;
            }
        }

        // =================================================
        // POSIÇÃO NORMAL DE SOLTAR
        // =================================================

        Vector3 origemDrop;

        if (cam != null)
        {
            origemDrop =
                cam.transform.position +
                cam.transform.forward *
                1.5f;
        }
        else
        {
            origemDrop =
                transform.position +
                transform.forward *
                1.5f;
        }

        // =================================================
        // PROCURAR O CHÃO
        // =================================================

        Ray ray =
            new Ray(
                origemDrop +
                Vector3.up * 5f,

                Vector3.down
            );

        RaycastHit hit;

        bool encontrouChao =
            Physics.Raycast(
                ray,
                out hit,
                20f,
                ~0,
                QueryTriggerInteraction.Ignore
            );

        Vector3 dropPosition;

        if (encontrouChao)
        {
            // =================================================
            // PEGAR COLLIDER DO LIXO
            // =================================================

            Collider lixoCollider =
                trash.GetComponentInChildren<Collider>();

            float alturaLixo =
                0.5f;

            if (lixoCollider != null)
            {
                alturaLixo =
                    lixoCollider.bounds.extents.y;
            }

            // =================================================
            // COLOCAR ACIMA DO CHÃO
            // =================================================

            dropPosition =
                hit.point +
                Vector3.up *
                (alturaLixo + 0.05f);
        }
        else
        {
            dropPosition =
                origemDrop;
        }

        // =================================================
        // TIRAR DA MÃO
        // =================================================

        trash.transform.SetParent(
            null,
            true
        );

        trash.transform.position =
            dropPosition;

        // =================================================
        // RIGIDBODY
        // =================================================

        Rigidbody rb =
            trash.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb =
                trash.GetComponentInChildren<Rigidbody>();
        }

        if (rb != null)
        {
            rb.isKinematic =
                false;

            rb.useGravity =
                true;

            rb.linearVelocity =
                Vector3.zero;

            rb.angularVelocity =
                Vector3.zero;
        }

        // =================================================
        // REATIVAR COLLIDERS
        // =================================================

        Collider[] cols =
            trash.GetComponentsInChildren<Collider>(
                true
            );

        foreach (Collider col in cols)
        {
            col.enabled =
                true;
        }

        // =================================================
        // SOM
        // =================================================

        TocarSomSoltar();

        Debug.Log(
            "🗑️ Lixo colocado no chão."
        );

        trash =
            null;
    }

    // =====================================================
    // LIXO BATEU NA PAREDE
    // =====================================================

    private void VoltarLixoDaParede(
        ref GameObject trash,
        Vector3 direcao)
    {
        if (trash == null)
            return;

        Debug.Log(
            "🧱 Lixo bateu na parede! Voltando para o jogador."
        );

        // =================================================
        // POSIÇÃO DE RETORNO
        // =================================================

        Vector3 voltarPosition =
            transform.position +
            transform.forward *
            distanciaVoltaParede +
            Vector3.up *
            alturaVoltaParede;

        // =================================================
        // TIRAR DA MÃO
        // =================================================

        trash.transform.SetParent(
            null,
            true
        );

        trash.transform.position =
            voltarPosition;

        // =================================================
        // RIGIDBODY
        // =================================================

        Rigidbody rb =
            trash.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb =
                trash.GetComponentInChildren<Rigidbody>();
        }

        if (rb != null)
        {
            rb.isKinematic =
                false;

            rb.useGravity =
                true;

            // =================================================
            // IMPULSO PARA TRÁS
            // =================================================

            rb.linearVelocity =
                -direcao *
                forcaVoltaParede;

            rb.angularVelocity =
                Vector3.zero;
        }

        // =================================================
        // REATIVAR COLLIDERS
        // =================================================

        Collider[] cols =
            trash.GetComponentsInChildren<Collider>(
                true
            );

        foreach (Collider col in cols)
        {
            col.enabled =
                true;
        }

        // =================================================
        // SOM
        // =================================================

        TocarSomSoltar();

        // =================================================
        // LIMPAR REFERÊNCIA
        // =================================================

        trash =
            null;
    }

    // =====================================================
    // COLOCAR NA LIXEIRA
    // =====================================================

    private void ColocarNaLixeira(
        ref GameObject trash)
    {
        if (trash == null)
            return;

        // =================================================
        // ENCONTRAR VALOR
        // =================================================

        TrashValue valor =
            EncontrarTrashValue(
                trash
            );

        // =================================================
        // PAGAMENTO
        // =================================================

        if (valor != null)
        {
            int dinheiroGanho =
                valor.ReceberValor();

            if (dinheiroGanho > 0)
            {
                if (playerMoney != null)
                {
                    playerMoney.AdicionarDinheiro(
                        dinheiroGanho
                    );
                }

                Debug.Log(
                    "💰 Você ganhou R$" +
                    dinheiroGanho +
                    " pelo lixo!"
                );
            }

            // =================================================
            // ATUALIZAR TAREFA
            // =================================================

            if (taskManager == null)
            {
                taskManager =
                    FindFirstObjectByType<TrashTaskManager>();
            }

            if (taskManager != null)
            {
                taskManager.LixoColocadoNaLixeira(
                    valor
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "⚠️ Esse lixo não possui TrashValue!"
            );
        }

        // =================================================
        // SOM
        // =================================================

        TocarSomLixeira();

        // =================================================
        // DESTRUIR
        // =================================================

        Destroy(
            trash
        );

        trash =
            null;

        Debug.Log(
            "🗑️ Lixo colocado na lixeira!"
        );
    }

    // =====================================================
    // ENCONTRAR TRASH VALUE
    // =====================================================

    private TrashValue EncontrarTrashValue(
        GameObject trash)
    {
        if (trash == null)
            return null;

        TrashValue valor =
            trash.GetComponent<TrashValue>();

        if (valor != null)
            return valor;

        valor =
            trash.GetComponentInParent<TrashValue>();

        if (valor != null)
            return valor;

        valor =
            trash.GetComponentInChildren<TrashValue>(
                true
            );

        return valor;
    }

    // =====================================================
    // SOM DE COLETA
    // =====================================================

    private void TocarSomColeta()
    {
        if (audioSource == null ||
            somColeta == null)
            return;

        audioSource.PlayOneShot(
            somColeta
        );
    }

    // =====================================================
    // SOM DE SOLTAR
    // =====================================================

    private void TocarSomSoltar()
    {
        if (audioSource == null ||
            somSoltar == null)
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
        if (audioSource == null ||
            somLixeira == null)
            return;

        audioSource.PlayOneShot(
            somLixeira
        );
    }
}