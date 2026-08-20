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
    // DISTÂNCIA DO DROP
    // =====================================================

    [Header("Distância do Drop")]
    public float distanciaDrop = 1.5f;

    // =====================================================
    // PROTEÇÃO CONTRA PAREDES
    // =====================================================

    [Header("Proteção contra paredes")]
    public float distanciaVerificacaoParede = 2.5f;

    public float distanciaVoltaParede = 0.8f;

    public float alturaVoltaParede = 1.0f;

    public float forcaVoltaParede = 2f;

    // =====================================================
    // UI
    // =====================================================

    [Header("Texto de Interação")]
    public bool mostrarTexto = true;

    public float larguraTexto = 420f;

    public float alturaTexto = 65f;

    public float distanciaTextoDoFundo = 120f;

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
    // CAMERA
    // =====================================================

    private Camera cam;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        playerMoney =
            GetComponent<PlayerMoney>();

        taskManager =
            FindFirstObjectByType<TrashTaskManager>();

        cam =
            Camera.main;

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

        if (cam == null)
            cam = Camera.main;

        // =================================================
        // Q = COLOCAR NA LIXEIRA
        // =================================================

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            TentarColocarNaLixeira();
        }

        // =================================================
        // E = PEGAR / DROPAR
        // =================================================

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            HandlePickupDrop();
        }
    }

    // =====================================================
    // PEGAR / DROPAR
    // =====================================================

    private void HandlePickupDrop()
    {
        if (cam == null)
            return;

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
        // NÃO ESTÁ OLHANDO PARA LIXO
        // E = DROPAR
        // =================================================

        DropAvailableTrash();
    }

    // =====================================================
    // TENTAR COLOCAR NA LIXEIRA
    // =====================================================

    private void TentarColocarNaLixeira()
    {
        if (cam == null)
            return;

        // =================================================
        // VERIFICAR SE TEM LIXO
        // =================================================

        if (leftTrash == null &&
            rightTrash == null)
        {
            Debug.Log(
                "❌ Você não está segurando nenhum lixo."
            );

            return;
        }

        // =================================================
        // RAYCAST DA CÂMERA
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
                ~0,
                QueryTriggerInteraction.Collide
            );

        System.Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(
                    b.distance
                )
        );

        // =================================================
        // PROCURAR LIXEIRA
        // =================================================

        foreach (RaycastHit hit in hits)
        {
            TrashBin bin =
                EncontrarTrashBin(
                    hit.collider
                );

            if (bin == null)
                continue;

            // =================================================
            // COLOCAR O LIXO
            // =================================================

            if (rightTrash != null)
            {
                ColocarNaLixeira(
                    ref rightTrash
                );

                return;
            }

            if (leftTrash != null)
            {
                ColocarNaLixeira(
                    ref leftTrash
                );

                return;
            }
        }

        Debug.Log(
            "❌ Você não está mirando em uma lixeira."
        );
    }

    // =====================================================
    // ENCONTRAR LIXEIRA
    // =====================================================

    private TrashBin EncontrarTrashBin(
        Collider collider)
    {
        if (collider == null)
            return null;

        TrashBin bin =
            collider.GetComponent<TrashBin>();

        if (bin != null)
            return bin;

        bin =
            collider.GetComponentInParent<TrashBin>();

        if (bin != null)
            return bin;

        bin =
            collider.GetComponentInChildren<TrashBin>(
                true
            );

        return bin;
    }

    // =====================================================
    // ENCONTRAR LIXO
    // =====================================================

    private GameObject EncontrarLixo(
        Collider collider)
    {
        if (collider == null)
            return null;

        if (collider.CompareTag("Trash"))
        {
            return collider.gameObject;
        }

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

        Vector3 escalaMundialOriginal =
            trash.transform.lossyScale;

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

        Collider[] cols =
            trash.GetComponentsInChildren<Collider>(
                true
            );

        foreach (Collider col in cols)
        {
            col.enabled =
                false;
        }

        trash.transform.SetParent(
            holdingPoint,
            true
        );

        trash.transform.position =
            holdingPoint.position;

        trash.transform.rotation =
            holdingPoint.rotation;

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

    // =====================================================
    // SOLTAR LIXO
    // =====================================================

    private void DropTrash(
        ref GameObject trash)
    {
        if (trash == null)
            return;

        Camera cameraAtual =
            Camera.main;

        Vector3 origem;

        Vector3 direcao;

        if (cameraAtual != null)
        {
            origem =
                cameraAtual.transform.position;

            direcao =
                cameraAtual.transform.forward;
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
        // PAREDE
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
            if (paredeHit.collider.transform.IsChildOf(
                transform))
            {
                bateuEmAlgo = false;
            }
        }

        if (bateuEmAlgo)
        {
            float verticalidade =
                Mathf.Abs(
                    Vector3.Dot(
                        paredeHit.normal,
                        Vector3.up
                    )
                );

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
        // DISTÂNCIA DO DROP
        // =================================================

        Vector3 origemDrop;

        if (cameraAtual != null)
        {
            origemDrop =
                cameraAtual.transform.position +
                cameraAtual.transform.forward *
                distanciaDrop;
        }
        else
        {
            origemDrop =
                transform.position +
                transform.forward *
                distanciaDrop;
        }

        // =================================================
        // CHÃO
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
            Collider lixoCollider =
                trash.GetComponentInChildren<Collider>();

            float alturaLixo =
                0.5f;

            if (lixoCollider != null)
            {
                alturaLixo =
                    lixoCollider.bounds.extents.y;
            }

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
        // COLLIDERS
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

        TocarSomSoltar();

        Debug.Log(
            "🗑️ Lixo colocado no chão."
        );

        trash =
            null;
    }

    // =====================================================
    // VOLTAR DA PAREDE
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

        Vector3 voltarPosition =
            transform.position +
            transform.forward *
            distanciaVoltaParede +
            Vector3.up *
            alturaVoltaParede;

        trash.transform.SetParent(
            null,
            true
        );

        trash.transform.position =
            voltarPosition;

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
                -direcao *
                forcaVoltaParede;

            rb.angularVelocity =
                Vector3.zero;
        }

        Collider[] cols =
            trash.GetComponentsInChildren<Collider>(
                true
            );

        foreach (Collider col in cols)
        {
            col.enabled =
                true;
        }

        TocarSomSoltar();

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

        TrashValue valor =
            EncontrarTrashValue(
                trash
            );

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

        TocarSomLixeira();

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
    // SOM COLETA
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
    // SOM SOLTAR
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
    // SOM LIXEIRA
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

    // =====================================================
    // UI
    // =====================================================

    private void OnGUI()
    {
        if (!mostrarTexto)
            return;

        if (cam == null)
            cam = Camera.main;

        if (cam == null)
            return;

        // =================================================
        // SE ESTÁ OLHANDO PARA UMA LIXEIRA
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
                ~0,
                QueryTriggerInteraction.Collide
            );

        System.Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(
                    b.distance
                )
        );

        foreach (RaycastHit hit in hits)
        {
            TrashBin bin =
                EncontrarTrashBin(
                    hit.collider
                );

            if (bin != null)
            {
                // =================================================
                // TEM LIXO = MOSTRAR Q
                // =================================================

                if (TemLixoNasMaos())
                {
                    MostrarTexto(
                        "[Q] JOGAR LIXO FORA"
                    );
                }

                return;
            }
        }

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
            // NÃO ESTÁ SEGURANDO = PEGAR
            // =================================================

            if (!TemLixoNasMaos())
            {
                MostrarTexto(
                    "[E] PEGAR LIXO"
                );

                return;
            }

            break;
        }

        // =================================================
        // ESTÁ SEGURANDO E NÃO ESTÁ NA LIXEIRA
        // =================================================

        if (TemLixoNasMaos())
        {
            MostrarTexto(
                "[E] DROPAR LIXO"
            );
        }
    }

    // =====================================================
    // TEM LIXO
    // =====================================================

    private bool TemLixoNasMaos()
    {
        return
            leftTrash != null ||
            rightTrash != null;
    }

    // =====================================================
    // MOSTRAR TEXTO
    // =====================================================

    private void MostrarTexto(
        string texto)
    {
        GUIStyle estilo =
            new GUIStyle(
                GUI.skin.box
            );

        estilo.fontSize =
            18;

        estilo.fontStyle =
            FontStyle.Bold;

        estilo.alignment =
            TextAnchor.MiddleCenter;

        GUI.Box(
            new Rect(
                (Screen.width - larguraTexto) / 2f,
                Screen.height - distanciaTextoDoFundo,
                larguraTexto,
                alturaTexto
            ),
            texto,
            estilo
        );
    }
}