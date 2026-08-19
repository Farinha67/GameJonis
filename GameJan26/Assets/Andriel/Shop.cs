using UnityEngine;
using UnityEngine.InputSystem;

public class Shop : MonoBehaviour
{
    // =====================================================
    // TIPO DE SEMENTE
    // =====================================================

    [System.Serializable]
    public class TipoSemente
    {
        public string nome;
        public int preco;
        public GameObject prefabArvore;
    }

    // =====================================================
    // SEMENTES
    // =====================================================

    [Header("🌱 Sementes")]
    public TipoSemente arvoreNivel1;
    public TipoSemente arvoreNivel2;
    public TipoSemente arvoreNivel3;
    public TipoSemente pinheiro;
    public TipoSemente macieira;

    // =====================================================
    // REGADOR
    // =====================================================

    [Header("💧 Regador")]
    public int precoRegador = 15;
    public GameObject regadorPrefab;
    public Transform holdingPoint;

    [Header("Tecla do Regador")]
    public Key teclaRegador = Key.R;

    // =====================================================
    // AJUSTE DA ÁRVORE NA MÃO
    // =====================================================

    [Header("🌳 Ajuste da Árvore na Mão")]

    [Tooltip("Posição da árvore em relação ao Holding Point.")]
    public Vector3 posicaoArvoreNaMao = Vector3.zero;

    [Tooltip("Rotação da árvore em relação ao Holding Point.")]
    public Vector3 rotacaoArvoreNaMao = Vector3.zero;

    [Tooltip("Tamanho da árvore na mão.")]
    public Vector3 escalaArvoreNaMao =
        new Vector3(0.15f, 0.15f, 0.15f);

    // =====================================================
    // AJUSTE DO REGADOR NA MÃO
    // =====================================================

    [Header("💧 Ajuste do Regador na Mão")]

    [Tooltip("Posição do regador em relação ao Holding Point.")]
    public Vector3 posicaoRegadorNaMao = Vector3.zero;

    [Tooltip("Rotação do regador em relação ao Holding Point.")]
    public Vector3 rotacaoRegadorNaMao = Vector3.zero;

    [Tooltip("Tamanho do regador na mão.")]
    public Vector3 escalaRegadorNaMao = Vector3.one;

    // =====================================================
    // CONFIGURAÇÃO
    // =====================================================

    [Header("⚙️ Configuração")]
    public float distanciaLoja = 3f;

    // =====================================================
    // SONS
    // =====================================================

    [Header("🔊 Sons da Loja")]
    public AudioSource audioSource;
    public AudioClip somCompraSemente;
    public AudioClip somCompraRegador;

    [Header("🔊 Som - Sem Dinheiro")]
    public AudioSource audioSourceSemDinheiro;
    public AudioClip somSemDinheiro;

    // =====================================================
    // PLAYER
    // =====================================================

    private Transform player;
    private PlayerMoney playerMoney;

    private bool playerPerto;

    // =====================================================
    // REGADOR
    // =====================================================

    private GameObject regadorNaMao;

    private bool possuiRegador;
    private bool regadorEquipado;

    // =====================================================
    // ÁRVORE NA MÃO
    // =====================================================

    private GameObject arvoreNaMao;

    // =====================================================
    // INVENTÁRIO
    // =====================================================

    private int quantidadeArvoreNivel1;
    private int quantidadeArvoreNivel2;
    private int quantidadeArvoreNivel3;
    private int quantidadePinheiro;
    private int quantidadeMacieira;

    // =====================================================
    // SEMENTE SELECIONADA
    // =====================================================

    private int sementeSelecionada = 0;

    /*
        0 = nenhuma
        1 = árvore nível 1
        2 = árvore nível 2
        3 = árvore nível 3
        4 = pinheiro
        5 = macieira
    */

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError(
                "❌ Player não encontrado! Confira a Tag Player."
            );

            return;
        }

        player =
            playerObject.transform;

        playerMoney =
            playerObject.GetComponent<PlayerMoney>();

        if (playerMoney == null)
        {
            Debug.LogError(
                "❌ Player não possui PlayerMoney!"
            );
        }

        // =================================================
        // HOLDING POINT
        // =================================================

        if (holdingPoint == null)
        {
            Debug.LogError(
                "❌ HOLDING POINT NÃO FOI CONFIGURADO NO SHOP!"
            );
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (player == null ||
            playerMoney == null)
            return;

        if (Keyboard.current == null)
            return;

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        playerPerto =
            distancia <= distanciaLoja;

        // =================================================
        // DENTRO DA LOJA
        // =================================================

        if (playerPerto)
        {
            // -------------------------------------------------
            // COMPRAR ÁRVORE 1
            // -------------------------------------------------

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    arvoreNivel1,
                    1
                );
            }

            // -------------------------------------------------
            // COMPRAR ÁRVORE 2
            // -------------------------------------------------

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    arvoreNivel2,
                    2
                );
            }

            // -------------------------------------------------
            // COMPRAR ÁRVORE 3
            // -------------------------------------------------

            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    arvoreNivel3,
                    3
                );
            }

            // -------------------------------------------------
            // COMPRAR PINHEIRO
            // -------------------------------------------------

            if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    pinheiro,
                    4
                );
            }

            // -------------------------------------------------
            // COMPRAR MACIEIRA
            // -------------------------------------------------

            if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    macieira,
                    5
                );
            }

            // -------------------------------------------------
            // COMPRAR REGADOR
            // -------------------------------------------------

            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                ComprarRegador();
            }

            return;
        }

        // =================================================
        // FORA DA LOJA
        // =================================================

        // -------------------------------------------------
        // SELECIONAR ÁRVORE 1
        // -------------------------------------------------

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SelecionarSemente(1);
        }

        // -------------------------------------------------
        // SELECIONAR ÁRVORE 2
        // -------------------------------------------------

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SelecionarSemente(2);
        }

        // -------------------------------------------------
        // SELECIONAR ÁRVORE 3
        // -------------------------------------------------

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            SelecionarSemente(3);
        }

        // -------------------------------------------------
        // SELECIONAR PINHEIRO
        // -------------------------------------------------

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            SelecionarSemente(4);
        }

        // -------------------------------------------------
        // SELECIONAR MACIEIRA
        // -------------------------------------------------

        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            SelecionarSemente(5);
        }

        // =================================================
        // R = PEGAR / GUARDAR REGADOR
        // =================================================

        if (Keyboard.current[teclaRegador].wasPressedThisFrame)
        {
            AlternarRegador();
        }
    }

    // =====================================================
    // COMPRAR SEMENTE
    // =====================================================

    private void ComprarSemente(
        TipoSemente semente,
        int numero
    )
    {
        if (semente == null)
            return;

        if (semente.prefabArvore == null)
        {
            Debug.LogError(
                "❌ Prefab da árvore não configurado: " +
                semente.nome
            );

            return;
        }

        if (!playerMoney.RemoverDinheiro(
                semente.preco))
        {
            TocarSomSemDinheiro();

            Debug.Log(
                "❌ Dinheiro insuficiente!"
            );

            Debug.Log(
                "💰 Dinheiro atual: R$" +
                playerMoney.GetDinheiro()
            );

            return;
        }

        AdicionarSemente(numero);

        if (audioSource != null &&
            somCompraSemente != null)
        {
            audioSource.PlayOneShot(
                somCompraSemente
            );
        }

        Debug.Log(
            "🌱 Comprou: " +
            semente.nome
        );

        Debug.Log(
            "📦 Quantidade: " +
            GetQuantidadeSemente(numero)
        );

        Debug.Log(
            "💰 Saldo: R$" +
            playerMoney.GetDinheiro()
        );
    }

    // =====================================================
    // ADICIONAR SEMENTE
    // =====================================================

    private void AdicionarSemente(int numero)
    {
        switch (numero)
        {
            case 1:
                quantidadeArvoreNivel1++;
                break;

            case 2:
                quantidadeArvoreNivel2++;
                break;

            case 3:
                quantidadeArvoreNivel3++;
                break;

            case 4:
                quantidadePinheiro++;
                break;

            case 5:
                quantidadeMacieira++;
                break;
        }
    }

    // =====================================================
    // SELECIONAR SEMENTE
    // =====================================================

    private void SelecionarSemente(int numero)
    {
        int quantidade =
            GetQuantidadeSemente(numero);

        if (quantidade <= 0)
        {
            Debug.Log(
                "❌ Você não possui essa semente."
            );

            return;
        }

        GameObject prefab =
            GetPrefabSemente(numero);

        if (prefab == null)
        {
            Debug.LogError(
                "❌ O prefab dessa árvore está vazio!"
            );

            return;
        }

        // =================================================
        // SALVAR SELEÇÃO
        // =================================================

        sementeSelecionada =
            numero;

        // =================================================
        // ESCONDER REGADOR
        // =================================================

        EsconderRegador();

        // =================================================
        // CRIAR ÁRVORE
        // =================================================

        CriarArvoreNaMao(
            prefab
        );

        Debug.Log(
            "🌳 Árvore equipada: " +
            GetNomeSemente(numero)
        );

        Debug.Log(
            "📦 Quantidade: " +
            quantidade
        );
    }

    // =====================================================
    // PEGAR PREFAB
    // =====================================================

    private GameObject GetPrefabSemente(
        int numero
    )
    {
        switch (numero)
        {
            case 1:
                return arvoreNivel1.prefabArvore;

            case 2:
                return arvoreNivel2.prefabArvore;

            case 3:
                return arvoreNivel3.prefabArvore;

            case 4:
                return pinheiro.prefabArvore;

            case 5:
                return macieira.prefabArvore;
        }

        return null;
    }

    // =====================================================
    // CRIAR ÁRVORE NA MÃO
    // =====================================================

    private void CriarArvoreNaMao(
        GameObject prefab
    )
    {
        if (holdingPoint == null)
        {
            Debug.LogError(
                "❌ HOLDING POINT ESTÁ VAZIO!"
            );

            return;
        }

        // =================================================
        // DESTRUIR ÁRVORE ANTERIOR
        // =================================================

        if (arvoreNaMao != null)
        {
            Destroy(
                arvoreNaMao
            );

            arvoreNaMao = null;
        }

        // =================================================
        // CRIAR ÁRVORE
        // =================================================

        arvoreNaMao =
            Instantiate(
                prefab
            );

        arvoreNaMao.name =
            "ARVORE_NA_MAO";

        // =================================================
        // COLOCAR NO HOLDING POINT
        // =================================================

        arvoreNaMao.transform.SetParent(
            holdingPoint,
            false
        );

        // =================================================
        // POSIÇÃO
        // =================================================

        arvoreNaMao.transform.localPosition =
            posicaoArvoreNaMao;

        // =================================================
        // ROTAÇÃO
        // =================================================

        arvoreNaMao.transform.localRotation =
            Quaternion.Euler(
                rotacaoArvoreNaMao
            );

        // =================================================
        // ESCALA
        // =================================================

        arvoreNaMao.transform.localScale =
            escalaArvoreNaMao;

        // =================================================
        // DESATIVAR RIGIDBODY
        // =================================================

        Rigidbody[] rigidbodies =
            arvoreNaMao.GetComponentsInChildren<Rigidbody>(
                true
            );

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // =================================================
        // DESATIVAR COLLIDERS
        // =================================================

        Collider[] colliders =
            arvoreNaMao.GetComponentsInChildren<Collider>(
                true
            );

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // =================================================
        // DESATIVAR PLANTSPOT
        // =================================================

        PlantSpot[] spots =
            arvoreNaMao.GetComponentsInChildren<PlantSpot>(
                true
            );

        foreach (PlantSpot spot in spots)
        {
            spot.enabled = false;
        }

        Debug.Log(
            "✅ Árvore criada na mão!"
        );
    }

    // =====================================================
    // ESCONDER ÁRVORE
    // =====================================================

    private void EsconderArvoreNaMao()
    {
        if (arvoreNaMao == null)
            return;

        Destroy(
            arvoreNaMao
        );

        arvoreNaMao = null;
    }

    // =====================================================
    // CONSUMIR SEMENTE
    // =====================================================

    public void ConsumirSemente()
    {
        if (sementeSelecionada == 0)
            return;

        switch (sementeSelecionada)
        {
            case 1:
                quantidadeArvoreNivel1--;
                break;

            case 2:
                quantidadeArvoreNivel2--;
                break;

            case 3:
                quantidadeArvoreNivel3--;
                break;

            case 4:
                quantidadePinheiro--;
                break;

            case 5:
                quantidadeMacieira--;
                break;
        }

        // =================================================
        // REMOVER ÁRVORE DA MÃO
        // =================================================

        EsconderArvoreNaMao();

        // =================================================
        // SE ACABOU A SEMENTE
        // =================================================

        if (GetQuantidadeSemente(
                sementeSelecionada
            ) <= 0)
        {
            sementeSelecionada = 0;
        }
    }

    // =====================================================
    // TEM SEMENTE SELECIONADA
    // =====================================================

    public bool TemSementeSelecionada()
    {
        if (sementeSelecionada == 0)
            return false;

        return
            GetQuantidadeSemente(
                sementeSelecionada
            ) > 0;
    }

    // =====================================================
    // PREFAB SELECIONADO
    // =====================================================

    public GameObject GetPrefabSementeSelecionada()
    {
        return GetPrefabSemente(
            sementeSelecionada
        );
    }

    // =====================================================
    // NÚMERO SELECIONADO
    // =====================================================

    public int GetSementeSelecionada()
    {
        return sementeSelecionada;
    }

    // =====================================================
    // NOME SELECIONADO
    // =====================================================

    public string GetNomeSementeSelecionada()
    {
        if (sementeSelecionada == 0)
            return "Nenhuma";

        return GetNomeSemente(
            sementeSelecionada
        );
    }

    private string GetNomeSemente(
        int numero
    )
    {
        switch (numero)
        {
            case 1:
                return arvoreNivel1.nome;

            case 2:
                return arvoreNivel2.nome;

            case 3:
                return arvoreNivel3.nome;

            case 4:
                return pinheiro.nome;

            case 5:
                return macieira.nome;
        }

        return "Nenhuma";
    }

    // =====================================================
    // QUANTIDADE
    // =====================================================

    public int GetQuantidadeSemente(
        int numero
    )
    {
        switch (numero)
        {
            case 1:
                return quantidadeArvoreNivel1;

            case 2:
                return quantidadeArvoreNivel2;

            case 3:
                return quantidadeArvoreNivel3;

            case 4:
                return quantidadePinheiro;

            case 5:
                return quantidadeMacieira;
        }

        return 0;
    }

    // =====================================================
    // COMPRAR REGADOR
    // =====================================================

    private void ComprarRegador()
    {
        if (possuiRegador)
        {
            MostrarRegador();

            Debug.Log(
                "💧 Regador equipado novamente."
            );

            return;
        }

        if (regadorPrefab == null)
        {
            Debug.LogError(
                "❌ Regador Prefab não configurado!"
            );

            return;
        }

        if (holdingPoint == null)
        {
            Debug.LogError(
                "❌ Holding Point não configurado!"
            );

            return;
        }

        if (!playerMoney.RemoverDinheiro(
                precoRegador))
        {
            TocarSomSemDinheiro();

            Debug.Log(
                "❌ Dinheiro insuficiente para o regador!"
            );

            return;
        }

        possuiRegador =
            true;

        CriarRegador();

        if (audioSource != null &&
            somCompraRegador != null)
        {
            audioSource.PlayOneShot(
                somCompraRegador
            );
        }

        Debug.Log(
            "💧 Regador comprado!"
        );
    }

    // =====================================================
    // CRIAR REGADOR
    // =====================================================

    private void CriarRegador()
    {
        if (holdingPoint == null)
        {
            Debug.LogError(
                "❌ Holding Point não configurado!"
            );

            return;
        }

        if (regadorNaMao != null)
        {
            Destroy(
                regadorNaMao
            );
        }

        // =================================================
        // INSTANCIAR
        // =================================================

        regadorNaMao =
            Instantiate(
                regadorPrefab
            );

        regadorNaMao.name =
            "REGADOR_NA_MAO";

        // =================================================
        // PARENT
        // =================================================

        regadorNaMao.transform.SetParent(
            holdingPoint,
            false
        );

        // =================================================
        // POSIÇÃO
        // =================================================

        regadorNaMao.transform.localPosition =
            posicaoRegadorNaMao;

        // =================================================
        // ROTAÇÃO
        // =================================================

        regadorNaMao.transform.localRotation =
            Quaternion.Euler(
                rotacaoRegadorNaMao
            );

        // =================================================
        // ESCALA
        // =================================================

        regadorNaMao.transform.localScale =
            escalaRegadorNaMao;

        // =================================================
        // RIGIDBODY
        // =================================================

        Rigidbody[] rigidbodies =
            regadorNaMao.GetComponentsInChildren<Rigidbody>(
                true
            );

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // =================================================
        // COLLIDERS
        // =================================================

        Collider[] colliders =
            regadorNaMao.GetComponentsInChildren<Collider>(
                true
            );

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // =================================================
        // ATIVAR
        // =================================================

        regadorNaMao.SetActive(
            true
        );

        regadorEquipado =
            true;

        Debug.Log(
            "💧 Regador criado na mão!"
        );
    }

    // =====================================================
    // ALTERNAR REGADOR
    // =====================================================

    private void AlternarRegador()
    {
        if (!possuiRegador)
        {
            Debug.Log(
                "❌ Você ainda não possui um regador."
            );

            return;
        }

        // =================================================
        // SE ESTIVER COM ÁRVORE
        // =================================================

        if (arvoreNaMao != null)
        {
            EsconderArvoreNaMao();

            sementeSelecionada =
                0;
        }

        // =================================================
        // CRIAR SE NECESSÁRIO
        // =================================================

        if (regadorNaMao == null)
        {
            CriarRegador();
            return;
        }

        // =================================================
        // ALTERNAR
        // =================================================

        if (regadorEquipado)
        {
            EsconderRegador();
        }
        else
        {
            MostrarRegador();
        }
    }

    // =====================================================
    // ESCONDER REGADOR
    // =====================================================

    private void EsconderRegador()
    {
        if (regadorNaMao == null)
            return;

        regadorNaMao.SetActive(
            false
        );

        regadorEquipado =
            false;
    }

    // =====================================================
    // MOSTRAR REGADOR
    // =====================================================

    private void MostrarRegador()
    {
        if (!possuiRegador)
            return;

        // =================================================
        // REMOVER ÁRVORE
        // =================================================

        EsconderArvoreNaMao();

        sementeSelecionada =
            0;

        // =================================================
        // CRIAR SE NECESSÁRIO
        // =================================================

        if (regadorNaMao == null)
        {
            CriarRegador();
            return;
        }

        // =================================================
        // PARENT
        // =================================================

        regadorNaMao.transform.SetParent(
            holdingPoint,
            false
        );

        // =================================================
        // APLICA AJUSTES NOVAMENTE
        // =================================================

        regadorNaMao.transform.localPosition =
            posicaoRegadorNaMao;

        regadorNaMao.transform.localRotation =
            Quaternion.Euler(
                rotacaoRegadorNaMao
            );

        regadorNaMao.transform.localScale =
            escalaRegadorNaMao;

        regadorNaMao.SetActive(
            true
        );

        regadorEquipado =
            true;

        Debug.Log(
            "💧 Regador equipado!"
        );
    }

    // =====================================================
    // REGADOR ESTÁ NA MÃO?
    // =====================================================

    public bool EstaSegurandoRegador()
    {
        return
            possuiRegador &&
            regadorNaMao != null &&
            regadorEquipado;
    }

    // =====================================================
    // USAR REGADOR
    // =====================================================

    public void UsarRegador()
    {
        if (!EstaSegurandoRegador())
            return;

        // O regador NÃO é destruído.
        Debug.Log(
            "💧 Regador utilizado!"
        );
    }

    // =====================================================
    // SOM SEM DINHEIRO
    // =====================================================

    private void TocarSomSemDinheiro()
    {
        if (audioSourceSemDinheiro != null &&
            somSemDinheiro != null)
        {
            audioSourceSemDinheiro.PlayOneShot(
                somSemDinheiro
            );
        }
    }

    // =====================================================
    // UI DA LOJA
    // =====================================================

    private void OnGUI()
    {
        if (!playerPerto ||
            playerMoney == null)
            return;

        GUIStyle estilo =
            new GUIStyle(
                GUI.skin.box
            );

        estilo.fontSize =
            18;

        estilo.alignment =
            TextAnchor.MiddleCenter;

        float largura =
            550f;

        float altura =
            390f;

        float x =
            (Screen.width - largura) / 2f;

        float y =
            Screen.height - 450f;

        string selecionada =
            sementeSelecionada == 0
                ? "Nenhuma"
                : GetNomeSemente(
                    sementeSelecionada
                );

        string estadoRegador =
            !possuiRegador
                ? "Não comprado"
                : regadorEquipado
                    ? "Equipado"
                    : "Guardado";

        string texto =
            "🛒 LOJA\n\n" +

            "1 - " +
            arvoreNivel1.nome +
            " - R$" +
            arvoreNivel1.preco +
            " [Qtd: " +
            quantidadeArvoreNivel1 +
            "]\n" +

            "2 - " +
            arvoreNivel2.nome +
            " - R$" +
            arvoreNivel2.preco +
            " [Qtd: " +
            quantidadeArvoreNivel2 +
            "]\n" +

            "3 - " +
            arvoreNivel3.nome +
            " - R$" +
            arvoreNivel3.preco +
            " [Qtd: " +
            quantidadeArvoreNivel3 +
            "]\n" +

            "4 - " +
            pinheiro.nome +
            " - R$" +
            pinheiro.preco +
            " [Qtd: " +
            quantidadePinheiro +
            "]\n" +

            "5 - " +
            macieira.nome +
            " - R$" +
            macieira.preco +
            " [Qtd: " +
            quantidadeMacieira +
            "]\n\n" +

            "Q - Comprar Regador - R$" +
            precoRegador +

            "\nR - Equipar / Guardar Regador" +

            "\n\n🌱 Selecionada: " +
            selecionada +

            "\n💧 Regador: " +
            estadoRegador +

            "\n💰 Dinheiro: R$" +
            playerMoney.GetDinheiro();

        GUI.Box(
            new Rect(
                x,
                y,
                largura,
                altura
            ),
            texto,
            estilo
        );
    }
}