using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickupTrash : MonoBehaviour
{
    [Header("Holding Points")]
    public Transform leftHoldingPoint;
    public Transform rightHoldingPoint;

    [Header("Pickup")]
    public float pickupDistance = 3f;

    [Header("Pagamento")]
    public int dinheiroPadraoPorLixo = 25;

    [Header("Tarefa")]
    public bool iniciarTarefaAutomaticamente = true;

    private GameObject leftTrash;
    private GameObject rightTrash;

    private PlayerMoney playerMoney;

    private int totalLixos;
    private int lixosColetados;

    private bool tarefaConcluida;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        playerMoney =
            GetComponent<PlayerMoney>();

        if (playerMoney == null)
        {
            Debug.LogError(
                "❌ PlayerPickupTrash: PlayerMoney não encontrado no Player!"
            );
        }

        if (iniciarTarefaAutomaticamente)
        {
            ContarLixos();
        }
    }

    // =====================================================
    // CONTAR LIXOS
    // =====================================================

    private void ContarLixos()
    {
        GameObject[] lixos =
            GameObject.FindGameObjectsWithTag("Trash");

        totalLixos =
            lixos.Length;

        lixosColetados = 0;

        tarefaConcluida = false;

        Debug.Log(
            "🗑️ Tarefa iniciada! Total de lixos: " +
            totalLixos
        );

        if (totalLixos == 0)
        {
            Debug.LogWarning(
                "⚠️ Nenhum objeto com a Tag 'Trash' foi encontrado!"
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
            return;

        Ray ray =
            new Ray(
                cam.transform.position,
                cam.transform.forward
            );

        // =================================================
        // VERIFICAR LIXO MIRADO
        // =================================================

        if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                pickupDistance))
        {
            GameObject trash =
                EncontrarLixo(hit.collider);

            if (trash != null)
            {
                // Não pegar o mesmo lixo duas vezes
                if (trash == leftTrash ||
                    trash == rightTrash)
                {
                    return;
                }

                // MÃO ESQUERDA
                if (leftTrash == null)
                {
                    leftTrash = trash;

                    AttachTrash(
                        leftTrash,
                        leftHoldingPoint
                    );

                    return;
                }

                // MÃO DIREITA
                if (rightTrash == null)
                {
                    rightTrash = trash;

                    AttachTrash(
                        rightTrash,
                        rightHoldingPoint
                    );

                    return;
                }

                Debug.Log(
                    "👐 As duas mãos estão ocupadas!"
                );

                return;
            }
        }

        // =================================================
        // NÃO ESTÁ MIRANDO EM LIXO
        // =================================================

        DropAvailableTrash();
    }

    // =====================================================
    // ENCONTRAR LIXO
    // =====================================================

    private GameObject EncontrarLixo(Collider collider)
    {
        if (collider == null)
            return null;

        if (collider.CompareTag("Trash"))
            return collider.gameObject;

        Transform parent =
            collider.transform.parent;

        while (parent != null)
        {
            if (parent.CompareTag("Trash"))
                return parent.gameObject;

            parent =
                parent.parent;
        }

        return null;
    }

    // =====================================================
    // COLOCAR NA MÃO
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

        Rigidbody rb =
            trash.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider[] colliders =
            trash.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
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

        Debug.Log(
            "🗑️ Lixo pegado!"
        );
    }

    // =====================================================
    // SOLTAR LIXO
    // =====================================================

    private void DropAvailableTrash()
    {
        // DIREITA PRIMEIRO
        if (rightTrash != null)
        {
            DropTrash(
                ref rightTrash
            );

            return;
        }

        // ESQUERDA
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
        // VERIFICAR LIXEIRAS
        // =================================================

        TrashBin[] trashBins =
            FindObjectsOfType<TrashBin>();

        foreach (TrashBin trashBin in trashBins)
        {
            if (trashBin == null)
                continue;

            if (trashBin.IsInside(
                    trash.transform.position))
            {
                ColocarNaLixeira(
                    ref trash
                );

                return;
            }
        }

        // =================================================
        // POSIÇÃO PARA SOLTAR
        // =================================================

        Camera cam =
            Camera.main;

        if (cam == null)
            return;

        Vector3 dropPosition =
            cam.transform.position +
            cam.transform.forward *
            0.5f;

        // =================================================
        // TIRAR DA MÃO
        // =================================================

        trash.transform.SetParent(
            null
        );

        trash.transform.position =
            dropPosition;

        // =================================================
        // REATIVAR FÍSICA
        // =================================================

        Rigidbody rb =
            trash.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // =================================================
        // REATIVAR COLISORES
        // =================================================

        Collider[] colliders =
            trash.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        trash = null;

        Debug.Log(
            "🗑️ Lixo solto."
        );
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
        // PEGAR VALOR
        // =================================================

        int valor =
            dinheiroPadraoPorLixo;

        TrashValue trashValue =
            trash.GetComponent<TrashValue>();

        if (trashValue != null)
        {
            valor =
                trashValue.dinheiro;
        }

        // =================================================
        // DAR DINHEIRO
        // =================================================

        if (playerMoney != null)
        {
            playerMoney.AdicionarDinheiro(
                valor
            );
        }

        // =================================================
        // CONTADOR
        // =================================================

        lixosColetados++;

        // =================================================
        // DESTRUIR
        // =================================================

        Destroy(
            trash
        );

        trash = null;

        Debug.Log(
            "🗑️ LIXO COLETADO!"
        );

        Debug.Log(
            "💰 Dinheiro ganho: R$" +
            valor
        );

        Debug.Log(
            "📦 Lixos coletados: " +
            lixosColetados +
            "/" +
            totalLixos
        );

        // =================================================
        // TERMINOU?
        // =================================================

        if (lixosColetados >= totalLixos)
        {
            tarefaConcluida =
                true;

            Debug.Log(
                "================================"
            );

            Debug.Log(
                "✅ TAREFA CONCLUÍDA!"
            );

            Debug.Log(
                "🗑️ Todos os lixos foram coletados!"
            );

            Debug.Log(
                "================================"
            );
        }
    }

    // =====================================================
    // UI DA TAREFA
    // =====================================================

    private void OnGUI()
    {
        if (tarefaConcluida)
        {
            MostrarTarefaConcluida();

            return;
        }

        if (totalLixos <= 0)
            return;

        MostrarTarefa();
    }

    // =====================================================
    // UI - TAREFA
    // =====================================================

    private void MostrarTarefa()
    {
        GUIStyle titulo =
            new GUIStyle(GUI.skin.label);

        titulo.fontSize = 20;
        titulo.fontStyle =
            FontStyle.Bold;

        GUIStyle texto =
            new GUIStyle(GUI.skin.label);

        texto.fontSize = 17;
        texto.fontStyle =
            FontStyle.Bold;

        GUI.Box(
            new Rect(
                20,
                20,
                300,
                115
            ),
            ""
        );

        GUI.Label(
            new Rect(
                35,
                30,
                270,
                30
            ),
            "🗑️ TAREFA",
            titulo
        );

        GUI.Label(
            new Rect(
                35,
                60,
                270,
                30
            ),
            "Colete todos os lixos",
            texto
        );

        GUI.Label(
            new Rect(
                35,
                88,
                270,
                30
            ),
            "Lixos: " +
            lixosColetados +
            "/" +
            totalLixos,
            texto
        );

        GUI.Label(
            new Rect(
                35,
                112,
                270,
                25
            ),
            "💰 Cada lixo dá dinheiro",
            texto
        );
    }

    // =====================================================
    // UI - CONCLUÍDA
    // =====================================================

    private void MostrarTarefaConcluida()
    {
        GUIStyle estilo =
            new GUIStyle(GUI.skin.box);

        estilo.fontSize = 20;

        estilo.fontStyle =
            FontStyle.Bold;

        estilo.alignment =
            TextAnchor.MiddleCenter;

        GUI.Box(
            new Rect(
                20,
                20,
                300,
                100
            ),
            "✅ TAREFA CONCLUÍDA!\n\n" +
            "🗑️ Todos os lixos foram coletados!"
        );
    }
}